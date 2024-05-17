using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class SeatRepository : Repository<Seat>
    {

        public SeatRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider, "Seat")
        {}

        public override async Task<int> CreateAsync(Seat entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Name, Status) VALUES (@Name, @Status)";
            RefreshCache();
            return await ExecuteAsync(sql, entity);
        }

        public override async Task UpdateAsync(Seat entity)
        {
            var sql = $"UPDATE [{TableName}] SET Name = @Name, Status = @Status WHERE Id = @Id";
            await ExecuteAsync(sql, entity);
            RefreshCache();
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @SeatId";
            await ExecuteAsync(sql, new { SeatId = id });
            RefreshCache();
        }
    }
}
