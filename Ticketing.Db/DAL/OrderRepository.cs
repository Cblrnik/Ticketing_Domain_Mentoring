using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class OrderRepository : Repository<Order>
    {
        private readonly Repository<Ticket> _ticketRepository;

        public OrderRepository(IConnectionStringProvider connectionStringProvider, Repository<Ticket> ticketRepository) : base(connectionStringProvider, "Order")
        {
            _ticketRepository = ticketRepository;
        }

        public override async Task<ICollection<Order>> GetAll()
        {
            await base.GetAll();

            foreach (var order in Entities)
            {
                order.Customer = await GetCustomerForOrder(order.Id).ConfigureAwait(false);
                order.Payment = await GetPaymentForOrder(order.Id).ConfigureAwait(false);
                order.Tickets = await GetTicketsForOrder(order.Id).ConfigureAwait(false);
            }

            return Entities;
        }

        public override async Task<int> Create(Order entity)
        {
            var sql = $"INSERT INTO [{TableName}] (CustomerId) VALUES (@CustomerId)";
            RefreshCache();
            var id = await ExecuteAsync(sql, new { CustomerId = entity.Customer?.Id });

            if (entity.Tickets != null) await UpdateTicketsOrderId(entity.Tickets, id);

            return id;
        }
       
        public override async Task Update(Order entity)
        {
            var sql = $"UPDATE [{TableName}] SET CustomerId = @CustomerId WHERE Id = @Id";
            RefreshCache();
            await ExecuteAsync(sql, new { CustomerId = entity.Customer?.Id, entity.Id });

            if (entity.Tickets != null) await UpdateTicketsOrderId(entity.Tickets, entity.Id);
        }

        public override async Task Delete(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });

            await ClearTicketsOrderId(id);
        }

        private async Task<Customer> GetCustomerForOrder(int orderId)
        {
            var sql = $"SELECT * FROM Customer WHERE Id IN (SELECT CustomerId FROM [{TableName}] WHERE Id = @OrderId)";
            return await QueryFirstOrDefaultAsync<Customer>(sql, new { OrderId = orderId });
        }

        private async Task<Payment> GetPaymentForOrder(int orderId)
        {
            var sql = "SELECT * FROM Payment WHERE OrderId = @OrderId)";
            return await QueryFirstOrDefaultAsync<Payment>(sql, new { OrderId = orderId });
        }

        private async Task<IEnumerable<Ticket>> GetTicketsForOrder(int orderId)
        {
            var sql = "SELECT Id FROM Ticket WHERE OrderId = @OrderId";
            var ids = await QueryAsync<int>(sql, new { OrderId = orderId });

            return (await _ticketRepository.GetAll()).Where(ticket => ids.Contains(ticket.Id));
        }

        private async Task ClearTicketsOrderId(int orderId)
        {
            var sql = "Update Ticket SET OrderId = null WHERE Id = @OrderId";
            await ExecuteAsync(sql, new { OrderId = orderId });
        }

        public async Task UpdateTicketsOrderId(IEnumerable<Ticket> tickets, int orderId)
        {
            await ClearTicketsOrderId(orderId);
            foreach (var ticket in tickets)
            {
                var sql = "Update Ticket SET OrderId = @OrderId WHERE Id = @Id";
                await ExecuteAsync(sql, new { OrderId = orderId, ticket.Id });
            }
        }
    }
}
