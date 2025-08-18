using System.Text.Json.Serialization;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Commands
{
    public class CreateMotorcycleCommand : ICommand<string>
    {
        [JsonPropertyName("identificador")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("ano")]
        public int Year { get; set; }

        [JsonPropertyName("modelo")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = string.Empty;
    }
}