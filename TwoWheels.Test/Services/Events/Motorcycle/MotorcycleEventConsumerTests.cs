using Microsoft.Extensions.Logging;
using System.Text.Json;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Events.Motorcycle;
using TwoWheels.Functions.Services.Events.Motorcycle.Models;

namespace TwoWheels.Test.Services.Events.Motorcycle
{
    public class MotorcycleEventConsumerTests
    {
        [Fact]
        public async Task Run_WithValidMotorcycle2024_ShouldSaveNotification()
        {
            // Arrange
            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            var mongoDbService = new Mock<IMongoDbService>();

            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                        .Returns(logger.Object);

            var consumer = new MotorcycleEventConsumer(loggerFactory.Object, mongoDbService.Object);

            var motorcycleEvent = new MotorcycleCreatedEvent
            {
                Id = "test-123",
                Year = 2024,
                Model = "Honda CB600",
                LicensePlate = "ABC1234"
            };

            var json = JsonSerializer.Serialize(motorcycleEvent);

            // Act
            await consumer.Run(json);

            // Assert
            mongoDbService.Verify(x => x.SaveNotificationAsync(It.IsAny<MotorcycleNotification>()), Times.Once);
        }

        [Fact]
        public async Task Run_WithMotorcycleNot2024_ShouldNotSaveNotification()
        {
            // Arrange
            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            var mongoDbService = new Mock<IMongoDbService>();

            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                        .Returns(logger.Object);

            var consumer = new MotorcycleEventConsumer(loggerFactory.Object, mongoDbService.Object);

            var motorcycleEvent = new MotorcycleCreatedEvent
            {
                Id = "test-456",
                Year = 2023,
                Model = "Yamaha MT-03",
                LicensePlate = "XYZ9876"
            };

            var json = JsonSerializer.Serialize(motorcycleEvent);

            // Act
            await consumer.Run(json);

            // Assert
            mongoDbService.Verify(x => x.SaveNotificationAsync(It.IsAny<MotorcycleNotification>()), Times.Never);
        }

        [Fact]
        public async Task Run_WithInvalidJson_ShouldThrowJsonException()
        {
            // Arrange
            var loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger>();
            var mongoDbService = new Mock<IMongoDbService>();

            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                        .Returns(logger.Object);

            var consumer = new MotorcycleEventConsumer(loggerFactory.Object, mongoDbService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => consumer.Run("invalid json"));
        }
    }
}