using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Rental.Queries
{
    public class GetRentalByIdQuery : IQuery<Domains.Entities.Rental>
    {
        public string Id { get; set; } = string.Empty;
    }
}