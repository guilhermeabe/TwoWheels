using System.Text.Json.Serialization;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Commands
{
    public class UpdateMotorcycleCommand : ICommand
    {
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = string.Empty;
    }
}
