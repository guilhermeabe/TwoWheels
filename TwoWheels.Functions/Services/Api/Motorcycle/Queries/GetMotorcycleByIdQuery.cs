using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Queries
{
    public class GetMotorcycleByIdQuery : IQuery<Domains.Entities.Motorcycle>
    {
        public string Id { get; set; } = string.Empty;
    }
}
