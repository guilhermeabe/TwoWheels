using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Commands
{
    public class UpdateMotorcycleCommand : ICommand
    {
        public string Id { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
    }
}
