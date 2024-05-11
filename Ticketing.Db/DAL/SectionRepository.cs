using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class SectionRepository : Repository<Section>
    {
        private readonly Repository<Seat> _seatRepository;

        public SectionRepository(IConnectionStringProvider connectionStringProvider, Repository<Seat> seatRepository) : base (connectionStringProvider, "Section")
        {
            _seatRepository = seatRepository;
        }

        public override async Task<ICollection<Section>> GetAllAsync()
        {
            await base.GetAllAsync();

            foreach (var section in Entities)
            {
                section.Seats = await GetSeatsForSection(section.Id).ConfigureAwait(false);
            }

            return Entities;
        }

        public override async Task<int> CreateAsync(Section entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Name) VALUES (@Name)";
            RefreshCache();
            entity.Id = await ExecuteAsync(sql, entity);

            if (entity.Seats != null) await CreateSeats(entity.Seats, entity.Id);
            return entity.Id;
        }

        public override async Task UpdateAsync(Section entity)
        {
            var sql = $"UPDATE [{TableName}] SET Name = @Name WHERE Id = @Id";
            await ExecuteAsync(sql, entity);
            RefreshCache();

            if (entity.Seats != null) await UpdateSeatsSection(entity.Seats, entity.Id);
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
            RefreshCache();

            await DeleteSeats(id);
        }

        private async Task<ICollection<Seat>> GetSeatsForSection(int id)
        {
            var sql = "SELECT * FROM Seat WHERE SectionId = @Id";
            return await QueryAsync<Seat>(sql, new { Id = id });
        }

        public async Task CreateSeats(IEnumerable<Seat> seats, int sectionId)
        {
            foreach (var seat in seats)
            {
                await _seatRepository.CreateAsync(seat);
            }

            await UpdateSeatsSection(seats, sectionId);
        }

        private async Task DeleteSeats(int id)
        {
            var sql = "SELECT Id FROM [Seat] WHERE VenueId = @Id";

            var ids = await QueryAsync<int>(sql, new { Id = id });
            foreach (var seatId in ids)
            {
                await _seatRepository.DeleteAsync(seatId);
            }
        }

        public async Task UpdateSeatsSection(IEnumerable<Seat> seats, int sectionId)
        {
            foreach (var seat in seats)
            {
                var sql = "UPDATE Seat SET SectionId = @SectionId WHERE Id = @Id";
                await ExecuteAsync(sql, new { SectionId = sectionId, seat.Id });
            }
        }
    }
}
