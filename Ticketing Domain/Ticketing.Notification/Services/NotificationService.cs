using System.Data.SqlClient;
using Dapper;
using Ticketing.Db.Models;

namespace Ticketing.Notification.Services
{
    public class NotificationService
    {
        private readonly string _connectionString =
            "Server=(localdb);Database=Ticketing;Trusted_Connection=True;MultipleActiveResultSets=true";

        public async Task UpdateNotificationStatusAsync(int trackingId, NotificationStatus status)
        {
            var notification = await GetByIdAsync(trackingId);

            notification.Status = status;

            await UpdateAsync(notification);
        }

        private async Task<int> UpdateAsync(Db.Models.Notification notification)
        {
            var sql = @"
                UPDATE Notifications 
                SET Status = @Status 
                WHERE TrackingId = @TrackingId"
            ;

            await using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return await connection.ExecuteAsync(sql, notification);
        }

        private async Task<Db.Models.Notification> GetByIdAsync(int trackingId)
        {
            var sql = $"SELECT * FROM [Notifications]";

            return (await QueryAsync<Db.Models.Notification>(sql).ConfigureAwait(false)).First(x => x.TrackingId == trackingId);
        }

        private async Task<ICollection<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return (await connection.QueryAsync<T>(sql, param)).ToList();
        }
    }
}
