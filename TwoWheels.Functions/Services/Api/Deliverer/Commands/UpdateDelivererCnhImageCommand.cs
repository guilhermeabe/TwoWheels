using System.Text.Json.Serialization;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Deliverer.Commands
{
    public class UpdateDelivererCnhImageCommand : ICommand
    {
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("imagem_cnh")]
        public string CnhImageBase64 { get; set; } = string.Empty;
    }
}