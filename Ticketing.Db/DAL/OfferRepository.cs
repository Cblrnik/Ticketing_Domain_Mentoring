using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class OfferRepository : Repository<Offer>
    {
        public OfferRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider, "Offer")
        {}

        public override async Task<ICollection<Offer>> GetAllAsync()
        {
            await base.GetAllAsync();

            foreach (var offer in Entities)
            {
                offer.Event = await GetEventForOffer(offer.Id).ConfigureAwait(false);
                offer.Seats = await GetSeatsForOffer(offer.Id).ConfigureAwait(false);
                offer.PriceLevels = await GetPriceLevelsForOffer(offer.Id).ConfigureAwait(false);
            }

            return Entities;
        }

        public override async Task<int> CreateAsync(Offer entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Name, EventId) VALUES (@Name, @EventId)";
            RefreshCache();
            var id = await ExecuteAsync(sql, new { entity.Name, entity.Event?.Id });

            if (entity.Seats != null) await CreateSeatsOffer(entity.Seats, id);
            if (entity.PriceLevels != null) await CreateTicketPriceLevelsOffer(entity.PriceLevels, id);

            return id;
        }

        public override async Task UpdateAsync(Offer entity)
        {
            var sql = $"UPDATE [{TableName}] SET Name = @Name, EventId = @EventId WHERE Id = @Id";
            await ExecuteAsync(sql, new { entity.Name, entity.Event?.Id });
            RefreshCache();

            if (entity.Seats != null) await CreateSeatsOffer(entity.Seats, entity.Id);
            if (entity.PriceLevels != null) await CreateTicketPriceLevelsOffer(entity.PriceLevels, entity.Id);
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
            RefreshCache();

            await ClearSeats(id);
            await ClearPriceLevels(id);
        }

        private async Task<Event> GetEventForOffer(int id)
        {
            var sql = $"SELECT * FROM Event WHERE Id IN (SELECT EventId FROM [{TableName}] WHERE Id = @Id)";
            return await QueryFirstOrDefaultAsync<Event>(sql, new { Id = id });
        }

        private async Task<ICollection<Seat>> GetSeatsForOffer(int id)
        {
            var sql = "SELECT * FROM Seat WHERE Id IN (SELECT SeatId FROM SeatOffers WHERE OfferId = @Id)";
            return await QueryAsync<Seat>(sql, new { Id = id });
        }

        private async Task<ICollection<TicketPriceLevel>> GetPriceLevelsForOffer(int id)
        {
            var sql = "SELECT * FROM TicketPriceLevel WHERE Id IN (SELECT PriceLevelId FROM OfferPriceLevels WHERE OfferId = @Id)";
            return await QueryAsync<TicketPriceLevel>(sql, new { Id = id });
        }

        public async Task CreateSeatsOffer(IEnumerable<Seat> seats, int offerId)
        {
            await ClearSeats(offerId);
            foreach (var seat in seats)
            {
                var sql = "INSERT INTO [SeatOffers] (OfferId, SeatId) VALUES (@Id, @SeatId)";
                await ExecuteAsync(sql, new { SeatId = seat.Id, Id = offerId });
            }
        }

        public async Task CreateTicketPriceLevelsOffer(IEnumerable<TicketPriceLevel> ticketPriceLevels, int offerId)
        {
            await ClearSeats(offerId);
            foreach (var ticketPriceLevel in ticketPriceLevels)
            {
                var sql = "INSERT INTO [OfferPriceLevels] (OfferId, PriceLevelId) VALUES (@Id, @TicketPriceLevelId)";
                await ExecuteAsync(sql, new { TicketPriceLevelId = ticketPriceLevel.Id, Id = offerId });
            }
        }

        private async Task ClearSeats(int id)
        {
            var sql = "DELETE FROM SeatOffers WHERE OfferId = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }

        public async Task ClearPriceLevels(int id)
        {
            var sql = "DELETE FROM OfferPriceLevels WHERE OfferId = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
    }
}
