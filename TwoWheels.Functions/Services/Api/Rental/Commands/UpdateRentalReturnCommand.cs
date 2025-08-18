using System.Text.Json.Serialization;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Rental.Commands
{
    public class UpdateRentalReturnCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("data_devolucao")]
        public DateTime ReturnDate { get; set; }
    }
}