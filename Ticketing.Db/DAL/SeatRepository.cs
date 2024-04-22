using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class SeatRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Seat";

        public SeatRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Seat> GetAllSeats()
        {
            const string sql = $"SELECT * FROM {TableName}";
            return _dataAccess.Query<Seat>(sql);
        }

        public Seat GetSeatById(int seatId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @SeatId";
            return _dataAccess.QueryFirstOrDefault<Seat>(sql, new { SeatId = seatId });
        }

        public void AddSeat(Seat seat)
        {
            const string sql = $"INSERT INTO {TableName} (Name, OfferId) VALUES (@Name, @OfferId)";
            _dataAccess.Execute(sql, new { seat.Name, OfferId = seat.Offer?.Id });
        }

        public void UpdateSeat(Seat seat)
        {
            const string sql = $"UPDATE {TableName} SET Name = @Name, OfferId = @OfferId WHERE Id = @Id";
            _dataAccess.Execute(sql, new { seat.Name, OfferId = seat.Offer?.Id, seat.Id });
        }

        public void DeleteSeat(int seatId)
        {
            const string sql = $"DELETE FROM {TableName} WHERE Id = @SeatId";
            _dataAccess.Execute(sql, new { SeatId = seatId });
        }
    }
}
