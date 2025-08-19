using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Events.Motorcycle.Models;

namespace TwoWheels.Functions.Services.Events.Motorcycle;

public class MotorcycleEventConsumer
{
    private readonly ILogger _logger;
    private readonly IMongoDbService _mongoDbService;

    public MotorcycleEventConsumer(ILoggerFactory loggerFactory, IMongoDbService mongoDbService)
    {
        _logger = loggerFactory.CreateLogger<MotorcycleEventConsumer>();
        _mongoDbService = mongoDbService;
    }

    [Function("MotorcycleEventConsumer")]
    public async Task Run([RabbitMQTrigger("%RabbitMQ_QueueName%", ConnectionStringSetting = "RabbitMQ_ConnectionString")] string motorCycleEvent)
    {
        try
        {
            _logger.LogInformation("Processing motorcycle event: {Message}", motorCycleEvent);

            var motorcycleEvent = JsonSerializer.Deserialize<MotorcycleCreatedEvent>(motorCycleEvent);

            if (motorcycleEvent == null)
            {
                _logger.LogWarning("Failed to deserialize motorcycle event: {Message}", motorCycleEvent);
                return;
            }

            _logger.LogInformation("Motorcycle event received - ID: {MotorcycleId}, Year: {Year}",
                motorcycleEvent.Id, motorcycleEvent.Year);

            if (motorcycleEvent.Year == 2024)
            {
                _logger.LogInformation("Processing motorcycle from 2024: {MotorcycleId}", motorcycleEvent.Id);

                var notification = new MotorcycleNotification
                {
                    Id = Guid.NewGuid().ToString(),
                    MotorcycleId = motorcycleEvent.Id,
                    Year = motorcycleEvent.Year,
                    Model = motorcycleEvent.Model,
                    LicensePlate = motorcycleEvent.LicensePlate,
                    EventType = motorcycleEvent.EventType,
                    NotificationReason = "Motorcycle registered in 2024",
                    CreatedAt = motorcycleEvent.CreatedAt,
                    ProcessedAt = DateTime.UtcNow
                };

                await _mongoDbService.SaveNotificationAsync(notification);

                _logger.LogInformation("Motorcycle notification saved successfully to MongoDB. " +
                    "MotorcycleId: {MotorcycleId}, NotificationId: {NotificationId}",
                    motorcycleEvent.Id, notification.Id);
            }
            else
            {
                _logger.LogInformation("Motorcycle not from 2024, skipping notification. " +
                    "MotorcycleId: {MotorcycleId}, Year: {Year}",
                    motorcycleEvent.Id, motorcycleEvent.Year);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing motorcycle event message: {Message}", motorCycleEvent);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing motorcycle event: {Message}", motorCycleEvent);
            throw;
        }
    }
}