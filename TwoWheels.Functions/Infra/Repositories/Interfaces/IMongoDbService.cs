using TwoWheels.Functions.Services.Events.Motorcycle.Models;

namespace TwoWheels.Functions.Infra.Repositories.Interfaces
{
    public interface IMongoDbService
    {
        Task SaveNotificationAsync(MotorcycleNotification notification);
    }
}