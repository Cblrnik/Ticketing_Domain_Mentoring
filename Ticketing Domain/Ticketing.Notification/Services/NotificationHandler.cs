using Confluent.Kafka;
using Newtonsoft.Json;
using Polly;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Db.Models;
using Ticketing.Notification.Models;

namespace Ticketing.Notification.Services
{
    public class NotificationHandler
    {
        private readonly string _bootstrapServers;
        private readonly string _emailProviderApiUrl;
        private readonly HttpClient _httpClient;
        private readonly NotificationService _notificationService;

        public NotificationHandler(HttpClient httpClient, NotificationService notificationService)
        {
            _bootstrapServers = "localhost:9092";
            _emailProviderApiUrl = "https://api.sendgrid.com/v3/mail/send";
            _httpClient = httpClient;
            _notificationService = notificationService;
        }

        public async Task ProcessNotificationQueue(string topic)
        {
            var config = new ConsumerConfig
            {
                GroupId = "notification-handler",
                BootstrapServers = _bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            while (true)
            {
                var consumeResult = consumer.Consume();
                var message = JsonConvert.DeserializeObject<NotificationMessage>(consumeResult.Message.Value);

                await _notificationService.UpdateNotificationStatusAsync(message.TrackingId, NotificationStatus.InProgress);

                await SendEmailRequestWithRetry(message);
            }
        }

        private async Task SendEmailRequestWithRetry(NotificationMessage message)
        {
            var policy = Policy.Handle<HttpRequestException>()
                               .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                               .RetryAsync(3, onRetry: (exception, retryCount) =>
                               {
                                   Console.WriteLine($"Повторная попытка {retryCount} для отправки email.");
                               });

            await policy.ExecuteAsync(async () =>
            {
                var emailRequest = new EmailRequest
                {
                    To = message.NotificationParameters?.CustomerEmail,
                    Subject = "Order Summary",
                    Body = message.NotificationContent?.OrderSummary
                };

                var response = await _httpClient.PostAsync(_emailProviderApiUrl,
                    new StringContent(JsonConvert.SerializeObject(emailRequest), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Email успешно отправлен.");
                    await _notificationService.UpdateNotificationStatusAsync(message.TrackingId, NotificationStatus.Success);
                }
                else
                {
                    Console.WriteLine("Ошибка отправки email.");
                    await _notificationService.UpdateNotificationStatusAsync(message.TrackingId, NotificationStatus.Failed);
                }

                return response;
            });
        }
    }
}
