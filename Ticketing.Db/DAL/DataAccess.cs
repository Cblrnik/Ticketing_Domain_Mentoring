using System.Data;
using System.Data.SqlClient;
using Dapper;


namespace Ticketing.Db.DAL
{
    public class DataAccess
    {
        private readonly string _connectionString;

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IEnumerable<T> Query<T>(string sql, object? param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            return connection.Query<T>(sql, param);
        }

        public T QueryFirstOrDefault<T>(string sql, object param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            return connection.QueryFirstOrDefault<T>(sql, param)!;
        }

        public int Execute(string sql, object? param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            return connection.Execute(sql, param);
        }
    }
}
