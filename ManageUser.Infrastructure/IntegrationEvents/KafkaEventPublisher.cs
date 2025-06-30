using Confluent.Kafka;
using ManageUser.Application.IntegrationEvents;
using System.Text.Json;

namespace ManageUser.Infrastructure.IntegrationEvents
{
    
    public class KafkaEventPublisher : IExternalEventPublisher
    {
        private readonly IProducer<string, string> _producer;

        public KafkaEventPublisher(IProducer<string, string> producer)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer), "Kafka producer cannot be null.");
        }

        public async Task PublishAsync<TEvent>(TEvent @event, string topic, CancellationToken cancellationToken)
        {
            var message = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event)
            };
            await _producer.ProduceAsync(topic, message, cancellationToken);
        }
    }
}
