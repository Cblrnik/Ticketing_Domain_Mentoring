using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class CustomerRepository : Repository<Customer>
    {

        public CustomerRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider,
            "Customer")
        {}

        public override async Task<int> Create(Customer entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Name, Email) VALUES (@Name, @Email)";
            RefreshCache();
            return await ExecuteAsync(sql, entity);
        }

        public override async Task Update(Customer entity)
        {
            var sql = $"UPDATE [{TableName}] SET Name = @Name, Email = @Email WHERE Id = @Id";
            await ExecuteAsync(sql, entity);
            RefreshCache();
        }

        public override async Task Delete(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @CustomerId";
            await ExecuteAsync(sql, new { CustomerId = id });
            RefreshCache();
        }
    }
}
