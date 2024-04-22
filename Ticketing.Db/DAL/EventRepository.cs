using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class EventRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Event";

        public EventRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Event> GetAllEvents()
        {
            const string sql = $"SELECT * FROM {TableName}";
            return _dataAccess.Query<Event>(sql);
        }

        public Event GetEventById(int eventId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @EventId";
            return _dataAccess.QueryFirstOrDefault<Event>(sql, new { EventId = eventId });
        }

        public void AddEvent(Event @event)
        {
            const string sql = $"INSERT INTO {TableName} (Name, Description, VenueId, StartDate, EndDate) VALUES (@Name, @Description, @VenueId, @StartDate, @EndDate)";
            _dataAccess.Execute(sql, @event);
        }

        public void UpdateEvent(Event @event)
        {
            const string sql = $"UPDATE {TableName} SET Name = @Name, Description = @Description, StartDate = @StartDate, EndDate = @EndDate WHERE Id = @Id";
            _dataAccess.Execute(sql, @event);
        }

        public void DeleteEvent(int eventId)
        {
            const string sql = $"DELETE FROM {TableName} WHERE Id = @EventId";
            _dataAccess.Execute(sql, new { EventId = eventId });
        }
    }
}
