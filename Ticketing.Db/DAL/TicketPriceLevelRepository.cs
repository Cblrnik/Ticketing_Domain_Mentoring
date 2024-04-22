using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class TicketPriceLevelRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "TicketPriceLevel";

        public TicketPriceLevelRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<TicketPriceLevel> GetAllTicketPriceLevels()
        {
            const string sql = $"SELECT * FROM {TableName}";
            return _dataAccess.Query<TicketPriceLevel>(sql);
        }

        public TicketPriceLevel GetTicketPriceLevelById(int ticketPriceLevelId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @TicketPriceLevelId";
            return _dataAccess.QueryFirstOrDefault<TicketPriceLevel>(sql, new { TicketPriceLevelId = ticketPriceLevelId });
        }

        public void AddTicketPriceLevel(TicketPriceLevel ticketPriceLevel)
        {
            const string sql = $"INSERT INTO {TableName} (Name, Price) VALUES (@Name, @Price)";
            _dataAccess.Execute(sql, ticketPriceLevel);
        }

        public void UpdateTicketPriceLevel(TicketPriceLevel ticketPriceLevel)
        {
            const string sql = $"UPDATE {TableName} SET Name = @Name, Price = @Price WHERE Id = @Id";
            _dataAccess.Execute(sql, ticketPriceLevel);
        }

        public void DeleteTicketPriceLevel(int ticketPriceLevelId)
        {
            const string sql = $"DELETE FROM {TableName} WHERE Id = @TicketPriceLevelId";
            _dataAccess.Execute(sql, new { TicketPriceLevelId = ticketPriceLevelId });
        }
    }
}
