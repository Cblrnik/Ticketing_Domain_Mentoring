using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class OrderRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Order";

        public OrderRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Order> GetAllOrders()
        {
            const string sql = $"SELECT * FROM {TableName}";
            var orders = _dataAccess.Query<Order>(sql).ToList();

            foreach (var order in orders)
            {
                order.Customer = GetCustomerForOrder(order.Id);
                order.Payment = GetPaymentForOrder(order.Id);
                order.Tickets = GetTicketsForOrder(order.Id);
            }

            return orders;
        }

        public Order GetOrderById(int orderId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @OrderId";
            var order = _dataAccess.QueryFirstOrDefault<Order>(sql, new { OrderId = orderId });

            order.Customer = GetCustomerForOrder(order.Id);
            order.Payment = GetPaymentForOrder(order.Id);
            order.Tickets = GetTicketsForOrder(order.Id);

            return order;
        }

        public void AddOrder(Order order)
        {
            const string sql = $"INSERT INTO {TableName} (CustomerId) VALUES (@CustomerId)";
            _dataAccess.Execute(sql, new { order.Customer.Id });

            var insertedOrderId = _dataAccess.QueryFirstOrDefault<int>("SELECT @@IDENTITY AS Id");

            if (order.Payment != null)
            {
                AddPaymentForOrder(order.Payment);
            }

            if (order.Tickets == null || !order.Tickets.Any()) return;
            foreach (var ticket in order.Tickets)
            {
                AddTicketForOrder(insertedOrderId, ticket);
            }
        }

        public void UpdateOrder(Order order)
        {
            const string sql = $"UPDATE {TableName} SET CustomerId = @CustomerId, PaymentId = @PaymentId WHERE Id = @Id";
            _dataAccess.Execute(sql, new { CustomerId = order.Customer?.Id, PaymentId = order.Payment?.Id, order.Id });

            if (order.Tickets == null || !order.Tickets.Any()) return;

            DeleteTicketsForOrder(order.Id);

            foreach (var ticket in order.Tickets)
            {
                AddTicketToOrder(order.Id, ticket);
            }
        }

        public void DeleteOrder(int orderId)
        {
            const string deleteOrderSql = $"DELETE FROM {TableName} WHERE Id = @OrderId";
            _dataAccess.Execute(deleteOrderSql, new { OrderId = orderId });

            DeleteTicketsForOrder(orderId);
        }

        private void AddTicketToOrder(int orderId, Ticket ticket)
        {
            const string sql = $"INSERT INTO OrderTickets (OrderId, TicketId) VALUES (@OrderId, @TicketId)";
            _dataAccess.Execute(sql, new { OrderId = orderId, TicketId = ticket.Id });
        }
        private void DeleteTicketsForOrder(int orderId)
        {
            const string deleteSql = "DELETE FROM OrderTickets WHERE OrderId = @OrderId";
            _dataAccess.Execute(deleteSql, new { OrderId = orderId });
        }

        private Customer GetCustomerForOrder(int orderId)
        {
            const string sql = $"SELECT * FROM Customer WHERE Id IN (SELECT CustomerId FROM {TableName} WHERE Id = @OrderId)";
            return _dataAccess.QueryFirstOrDefault<Customer>(sql, new { OrderId = orderId });
        }

        private Payment GetPaymentForOrder(int orderId)
        {
            const string sql = $"SELECT * FROM Payment WHERE Id IN (SELECT PaymentId FROM {TableName} WHERE Id = @OrderId)";
            return _dataAccess.QueryFirstOrDefault<Payment>(sql, new { OrderId = orderId });
        }

        private IEnumerable<Ticket> GetTicketsForOrder(int orderId)
        {
            const string sql = $"SELECT * FROM Ticket WHERE Id IN (SELECT TicketId FROM OrderTickets WHERE OrderId = @OrderId)";
            return _dataAccess.Query<Ticket>(sql, new { OrderId = orderId });
        }

        private void AddPaymentForOrder(Payment payment)
        {
            const string sql = "INSERT INTO Payment (Status, Amount) VALUES (0, @Amount)";
            _dataAccess.Execute(sql, new { payment.Amount });
        }

        private void AddTicketForOrder(int orderId, Ticket ticket)
        {
            const string sql = "INSERT INTO OrderTickets (OrderId, TicketId) VALUES (@OrderId, @TicketId)";
            _dataAccess.Execute(sql, new { OrderId = orderId, TicketId = ticket.Id });
        }
    }
}
