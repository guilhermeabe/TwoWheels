using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Events.Motorcycle.Models;

namespace TwoWheels.Functions.Infra.Repositories
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<MotorcycleNotification> _notificationsCollection;
        private readonly ILogger<MongoDbService> _logger;

        public MongoDbService(ILogger<MongoDbService> logger)
        {
            _logger = logger;

            var connectionString = Environment.GetEnvironmentVariable("MongoDB_ConnectionString")
                ?? string.Empty;
            var databaseName = Environment.GetEnvironmentVariable("MongoDB_DatabaseName")
                ?? string.Empty;
            var collectionName = Environment.GetEnvironmentVariable("MongoDB_NotificationsCollection")
                ?? string.Empty;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _notificationsCollection = database.GetCollection<MotorcycleNotification>(collectionName);
        }

        public async Task SaveNotificationAsync(MotorcycleNotification notification)
        {
            try
            {
                await _notificationsCollection.InsertOneAsync(notification);
                _logger.LogInformation("Notification saved to MongoDB. NotificationId: {NotificationId}", notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving notification to MongoDB. NotificationId: {NotificationId}", notification.Id);
                throw;
            }
        }
    }
}
