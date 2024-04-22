using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class VenueRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Venue";

        public VenueRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Venue> GetAllVenues()
        {
            const string sql = $"SELECT * FROM {TableName}";
            var venues = _dataAccess.Query<Venue>(sql).ToList();

            return venues;
        }

        public Venue GetVenueById(int venueId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @VenueId";
            var venue = _dataAccess.QueryFirstOrDefault<Venue>(sql, new { VenueId = venueId });

            return venue;
        }

        public void AddVenue(Venue venue)
        {
            const string sql = $"INSERT INTO {TableName} (Name, Address) VALUES (@Name, @Address)";
            _dataAccess.Execute(sql, venue);
        }

        public void UpdateVenue(Venue venue)
        {
            const string sql = $"UPDATE {TableName} SET Name = @Name, Address = @Address WHERE Id = @Id";
            _dataAccess.Execute(sql, venue);
        }

        public void DeleteVenue(int venueId)
        {
            const string deleteVenueSql = $"DELETE FROM {TableName} WHERE Id = @VenueId";
            _dataAccess.Execute(deleteVenueSql, new { VenueId = venueId });
        }

    }
}
