using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class PaymentRepository : Repository<Payment>
    {

        public PaymentRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider,
            "Payment")
        {}

        public override async Task<int> CreateAsync(Payment entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Status, Amount) VALUES (@Status, @Amount)";
            RefreshCache();
            return await ExecuteAsync(sql, entity);
        }

        public override async Task UpdateAsync(Payment entity)
        {
            var sql = $"UPDATE [{TableName}] SET Status = @Status, Amount = @Amount WHERE Id = @Id";
            await ExecuteAsync(sql, entity);
            RefreshCache();
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @PaymentId";
            await ExecuteAsync(sql, new { CustomerId = id });
            RefreshCache();
        }
    }
}
