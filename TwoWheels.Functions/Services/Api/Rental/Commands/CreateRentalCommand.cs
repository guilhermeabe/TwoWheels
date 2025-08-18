using System.Text.Json.Serialization;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Rental.Commands
{
    public class CreateRentalCommand : ICommand<string>
    {
        [JsonPropertyName("entregador_id")]
        public string DelivererId { get; set; } = string.Empty;

        [JsonPropertyName("moto_id")]
        public string MotorcycleId { get; set; } = string.Empty;

        [JsonPropertyName("data_inicio")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("data_termino")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("data_previsao_termino")]
        public DateTime ExpectedEndDate { get; set; }

        [JsonPropertyName("plano")]
        public int PlanDays { get; set; }
    }
}