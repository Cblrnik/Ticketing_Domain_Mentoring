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
            Entities = GetAll().Result;
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

        protected async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            return (await connection.QueryFirstOrDefaultAsync<T>(sql, param))!;
        }

        protected async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            using var connection = GetConnection();
            connection.Open();
            return await connection.ExecuteAsync(sql, param);
        }


        public async Task<T> GetById(int id)
        {
            if (IsUpdated)
            {
                await GetAll();
            }

            return Entities.FirstOrDefault(entity => entity.Id == id)!;
        }

        public virtual async Task<ICollection<T>> GetAll()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Entities != null && !IsUpdated) { return Entities; }

            var sql = $"SELECT * FROM [{TableName}]";

            Entities = await QueryAsync<T>(sql);
            IsUpdated = false;
            return Entities;
        }

        public abstract Task<int> Create(T entity);

        public abstract Task Update(T entity);

        public abstract Task Delete(int id);

        public void RefreshCache()
        {
            IsUpdated = true;
        }
    }
}
