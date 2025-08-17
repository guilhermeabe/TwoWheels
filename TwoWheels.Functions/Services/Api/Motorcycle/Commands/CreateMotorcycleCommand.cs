using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Commands
{
    public class CreateMotorcycleCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
    }
}
