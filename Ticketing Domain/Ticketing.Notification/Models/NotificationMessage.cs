using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.Notification.Models
{
    public class NotificationMessage
    {
        public int TrackingId { get; set; }
        public string? OperationName { get; set; }
        public DateTime Timestamp { get; set; }
        public NotificationParameters? NotificationParameters { get; set; }
        public NotificationContent? NotificationContent { get; set; }
    }

    public class NotificationParameters
    {
        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
    }

    public class NotificationContent
    {
        public double OrderAmount { get; set; }
        public string? OrderSummary { get; set; }
    }

    public class EmailRequest
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
}
