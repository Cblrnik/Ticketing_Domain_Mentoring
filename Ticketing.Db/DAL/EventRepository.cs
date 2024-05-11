using System.Xml.Linq;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class EventRepository : Repository<Event>
    {
        public EventRepository(IConnectionStringProvider connectionStringProvider) : base(connectionStringProvider, "Event")
        {}

        public override async Task<int> CreateAsync(Event entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Name, Description, VenueId, StartDate, EndDate) VALUES (@Name, @Description, @VenueId, @StartDate, @EndDate)";
            RefreshCache();
            
            entity.Id = await ExecuteAsync(sql, new
            {
                entity.Name,
                entity.Description,
                VenueId = entity.Venue?.Id,
                entity.StartDate,
                entity.EndDate
            });
            return entity.Id;
        }

        public override async Task UpdateAsync(Event entity)
        {
            var sql = $"UPDATE [{TableName}] SET Name = @Name, Description = @Description, VenueId = @VenueId, StartDate = @StartDate, EndDate = @EndDate WHERE Id = @Id";
            await ExecuteAsync(sql, new
            {
                entity.Name,
                entity.Description,
                VenueId = entity.Venue?.Id,
                entity.StartDate,
                entity.EndDate,
                entity.Id
            });
            RefreshCache();
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @EventId";
            await ExecuteAsync(sql, new { EventId = id });
            RefreshCache();
        }
    }
}
