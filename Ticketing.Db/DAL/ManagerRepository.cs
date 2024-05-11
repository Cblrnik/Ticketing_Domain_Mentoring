using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class ManagerRepository : Repository<Manager>
    {
        public ManagerRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider, "Manager")
        {}

        public override async Task<int> CreateAsync(Manager entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Id, Name, Login, Password) VALUES (@Id, @Name, @Login, @Password)";
            RefreshCache();
            return await ExecuteAsync(sql, entity);
        }

        public override async Task UpdateAsync(Manager entity)
        {
            var sql = $"UPDATE [{TableName}] SET Name = @Name, Login = @Login, Password = @Password WHERE Id = @Id";
            await ExecuteAsync(sql, entity);
            RefreshCache();
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @ManagerId";
            await ExecuteAsync(sql, new { CustomerId = id });
            RefreshCache();
        }
    }
}
