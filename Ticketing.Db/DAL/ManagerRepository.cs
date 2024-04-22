using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class ManagerRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Manager";

        public ManagerRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Manager> GetAllManagers()
        {
            const string sql = $"SELECT * FROM {TableName}";
            return _dataAccess.Query<Manager>(sql);
        }

        public Manager GetManagerById(Guid managerId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @ManagerId";
            return _dataAccess.QueryFirstOrDefault<Manager>(sql, new { ManagerId = managerId });
        }

        public void AddManager(Manager manager)
        {
            const string sql = $"INSERT INTO {TableName} (Id, Name, Login, Password) VALUES (@Id, @Name, @Login, @Password)";
            _dataAccess.Execute(sql, manager);
        }

        public void UpdateManager(Manager manager)
        {
            const string sql = $"UPDATE {TableName} SET Name = @Name, Login = @Login, Password = @Password WHERE Id = @Id";
            _dataAccess.Execute(sql, manager);
        }

        public void DeleteManager(Guid managerId)
        {
            const string sql = $"DELETE FROM {TableName} WHERE Id = @ManagerId";
            _dataAccess.Execute(sql, new { ManagerId = managerId });
        }
    }
}
