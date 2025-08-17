using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Queries
{
    public class GetMotorcyclesQuery : IQuery<List<Domains.Entities.Motorcycle>>
    {
        public string? LicensePlate { get; set; }
    }
}
