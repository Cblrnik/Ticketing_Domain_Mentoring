using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class TicketRepository : Repository<Ticket>
    {

        public TicketRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider, "Ticket")
        {}

        public override async Task<ICollection<Ticket>> GetAll()
        {
            await base.GetAll();

            foreach (var ticket in Entities)
            {
                ticket.Event = await GetEventForTicket(ticket.Id).ConfigureAwait(false);
                ticket.Seat = await GetSeatForOrder(ticket.Id).ConfigureAwait(false);
            }

            return Entities;
        }

        public override async Task<int> Create(Ticket entity)
        {
            var sql = $"INSERT INTO [{TableName}] (EventId, SeatId, OrderId, Status) VALUES (@EventId, @SeatId, null, 0)";
            RefreshCache();
            var id = await ExecuteAsync(sql, new { EventId = entity.Event?.Id, SeatId = entity.Seat?.Id});

            return id;
        }

        public override async Task Update(Ticket entity)
        {
            var sql = $"UPDATE [{TableName}] SET EventId = @EventId, SeatId = @SeatId, Status = @Status WHERE Id = @Id";
            RefreshCache();
            await ExecuteAsync(sql, new { EventId = entity.Event?.Id, SeatId = entity.Seat?.Id, entity.Status, entity.Id });
        }

        public override async Task Delete(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }


        private async Task<Event> GetEventForTicket(int id)
        {
            var sql = $"SELECT * FROM Event WHERE Id IN (SELECT EventId FROM [{TableName}] WHERE Id = @Id)";
            return await QueryFirstOrDefaultAsync<Event>(sql, new { Id = id });
        }

        private async Task<Seat> GetSeatForOrder(int id)
        {
            var sql = $"SELECT * FROM Seat WHERE Id IN (SELECT EventId FROM [{TableName}] WHERE Id = @Id)";
            return await QueryFirstOrDefaultAsync<Seat>(sql, new { Id = id });
        }
    }
}
