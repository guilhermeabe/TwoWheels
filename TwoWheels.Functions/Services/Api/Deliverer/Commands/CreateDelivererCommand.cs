using System.Text.Json.Serialization;
using TwoWheels.Functions.Domains.Enuns;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Deliverer.Commands
{
    public class CreateDelivererCommand : ICommand<string>
    {
        [JsonPropertyName("identificador")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [JsonPropertyName("data_nascimento")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = string.Empty;

        [JsonPropertyName("tipo_cnh")]
        public string CnhTypeString { get; set; } = string.Empty;

        [JsonPropertyName("imagem_cnh")]
        public string? CnhImageBase64 { get; set; }

        public CnhType CnhType
        {
            get
            {
                return CnhTypeString.ToUpperInvariant() switch
                {
                    "A" => CnhType.A,
                    "B" => CnhType.B,
                    "A+B" => CnhType.AB,
                    "AB" => CnhType.AB,
                    _ => throw new ArgumentException($"Invalid CNH type: {CnhTypeString}")
                };
            }
        }
    }
}