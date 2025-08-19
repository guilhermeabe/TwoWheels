using TwoWheels.Functions.Services.Events.Motorcycle.Models;

namespace TwoWheels.Functions.Services.Events.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishMotorcycleCreatedAsync(MotorcycleCreatedEvent motorcycleEvent);
    }
}