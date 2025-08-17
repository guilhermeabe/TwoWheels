using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Commands
{
    public class DeleteMotorcycleCommand : ICommand
    {
        public string Id { get; set; } = string.Empty;
    }
}
