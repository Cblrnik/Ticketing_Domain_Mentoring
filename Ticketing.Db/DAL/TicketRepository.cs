using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class TicketRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Ticket";

        public TicketRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Ticket> GetAllTickets()
        {
            const string sql = $"SELECT * FROM {TableName}";
            return _dataAccess.Query<Ticket>(sql);
        }

        public Ticket GetTicketById(Guid ticketId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @TicketId";
            return _dataAccess.QueryFirstOrDefault<Ticket>(sql, new { TicketId = ticketId });
        }

        public void AddTicket(Ticket ticket)
        {
            const string sql = $"INSERT INTO {TableName} (CustomerId, EventId, SeatId, SectionId, Status) VALUES (@CustomerId, @EventId, @SeatId, @SectionId, @Status)";
            _dataAccess.Execute(sql, new { CustomerId = ticket.Customer?.Id, EventId = ticket.Event?.Id, SeatId = ticket.Seat?.Id, SectionId = ticket.Section?.Id, ticket.Status });
        }

        public void UpdateTicket(Ticket ticket)
        {
            const string sql = $"UPDATE {TableName} SET CustomerId = @CustomerId, EventId = @EventId, SeatId = @SeatId, SectionId = @SectionId, Status = @Status WHERE Id = @Id";
            _dataAccess.Execute(sql, new { CustomerId = ticket.Customer?.Id, EventId = ticket.Event?.Id, SeatId = ticket.Seat?.Id, SectionId = ticket.Section?.Id, ticket.Status,
                ticket.Id });
        }

        public void DeleteTicket(Guid ticketId)
        {
            const string sql = $"DELETE FROM {TableName} WHERE Id = @TicketId";
            _dataAccess.Execute(sql, new { TicketId = ticketId });
        }
    }
}
