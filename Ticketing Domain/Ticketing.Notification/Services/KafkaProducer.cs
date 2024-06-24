using Confluent.Kafka;
using Newtonsoft.Json;
using Ticketing.Notification.Models;

namespace Ticketing.Notification.Services
{
    public class KafkaProducer
    {
        private readonly string _bootstrapServers;

        public KafkaProducer()
        {
            _bootstrapServers = "localhost:9092";
        }

        public async Task SendMessageAsync(string topic, NotificationMessage message)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var jsonMessage = JsonConvert.SerializeObject(message);
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });
            producer.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
