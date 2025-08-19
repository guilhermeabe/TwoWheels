using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TwoWheels.Functions.Services.Events.Interfaces;
using TwoWheels.Functions.Services.Events.Motorcycle.Models;

namespace TwoWheels.Functions.Services.Events
{
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection? _connection;
        private readonly IChannel? _channel;
        private readonly string _queueName;
        private bool _disposed;

        public RabbitMqEventPublisher()
        {
            _queueName = Environment.GetEnvironmentVariable("RabbitMQ_QueueName") ?? string.Empty;

            try
            {
                var connectionString = Environment.GetEnvironmentVariable("RabbitMQ_ConnectionString") ?? string.Empty;

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(connectionString)
                };

                _connection = factory.CreateConnectionAsync().Result;
                _channel = _connection.CreateChannelAsync().Result;

                _channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                ).Wait();
            }
            catch (Exception)
            {
                _connection = null;
                _channel = null;
            }
        }

        public async Task PublishMotorcycleCreatedAsync(MotorcycleCreatedEvent motorcycleEvent)
        {
            if (_channel == null) return;

            var json = JsonSerializer.Serialize(motorcycleEvent);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true,
                MessageId = motorcycleEvent.EventId,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _queueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _channel?.Dispose();
                    _connection?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}