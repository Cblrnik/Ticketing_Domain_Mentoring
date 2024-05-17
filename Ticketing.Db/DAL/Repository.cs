using System.Data.SqlClient;
using System.Data;
using Dapper;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public abstract class Repository<T> where T : IIdentity
    {
        protected ICollection<T> Entities;
        protected readonly string TableName;
        private readonly string _connectionString;
        protected bool IsUpdated = false;

        protected Repository(IConnectionStringProvider connectionStringProvider, string tableName)
        {
            TableName = tableName;
            _connectionString = connectionStringProvider.GetConnectionString();
            Entities = GetAllAsync().Result;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected async Task<ICollection<Y>> QueryAsync<Y>(string sql, object? param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            return (await connection.QueryAsync<Y>(sql, param)).ToList();
        }

        protected async Task<Y> QueryFirstOrDefaultAsync<Y>(string sql, object? param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            return (await connection.QueryFirstOrDefaultAsync<Y>(sql, param))!;
        }

        protected async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            return await connection.ExecuteAsync(sql, param);
        }


        public virtual async Task<T> GetByIdAsync(int id)
        {
            if (IsUpdated)
            {
                await GetAllAsync();
            }

            return Entities.FirstOrDefault(entity => entity.Id == id)!;
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Entities != null && !IsUpdated) { return Entities; }

            var sql = $"SELECT * FROM [{TableName}]";

            Entities = await QueryAsync<T>(sql);
            IsUpdated = false;
            return Entities;
        }

        public abstract Task<int> CreateAsync(T entity);

        public abstract Task UpdateAsync(T entity);

        public abstract Task DeleteAsync(int id);

        public void RefreshCache()
        {
            IsUpdated = true;
        }
    }
}
