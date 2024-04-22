using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class CustomerRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Customer";

        public CustomerRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            const string sql = $"SELECT * FROM {TableName}";
            return _dataAccess.Query<Customer>(sql);
        }

        public Customer GetCustomerById(int customerId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @CustomerId";
            return _dataAccess.QueryFirstOrDefault<Customer>(sql, new { CustomerId = customerId });
        }

        public void AddCustomer(Customer customer)
        {
            const string sql = $"INSERT INTO {TableName} (Name, Email) VALUES (@Name, @Email)";
            _dataAccess.Execute(sql, customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            const string sql = $"UPDATE {TableName} SET Name = @Name, Email = @Email WHERE Id = @Id";
            _dataAccess.Execute(sql, customer);
        }

        public void DeleteCustomer(int customerId)
        {
            const string sql = $"DELETE FROM {TableName} WHERE Id = @CustomerId";
            _dataAccess.Execute(sql, new { CustomerId = customerId });
        }
    }
}
