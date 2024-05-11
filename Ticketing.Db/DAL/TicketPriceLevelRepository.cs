using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class TicketPriceLevelRepository : Repository<TicketPriceLevel>
    {

        public TicketPriceLevelRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider, "TicketPriceLevel") 
        {}

        public override async Task<int> CreateAsync(TicketPriceLevel entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Name, Price) VALUES (@Name, @Price)";
            RefreshCache();
            return await ExecuteAsync(sql, entity);
        }

        public override async Task UpdateAsync(TicketPriceLevel entity)
        {
            var sql = $"UPDATE [{TableName}] SET Name = @Name, Price = @Price WHERE Id = @Id";
            await ExecuteAsync(sql, entity);
            RefreshCache();
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @TicketPriceLevelId";
            await ExecuteAsync(sql, new { TicketPriceLevelId = id });
            RefreshCache();
        }
    }
}
