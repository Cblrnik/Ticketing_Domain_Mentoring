using Ticketing.Notification.Services;
var httpClient = new HttpClient();

var handler = new NotificationHandler(httpClient, new NotificationService());

await handler.ProcessNotificationQueue("ProcessingTask");