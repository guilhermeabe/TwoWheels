using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Motorcycle.Queries;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Handlers
{
    public class GetMotorcyclesHandler : IRequestHandler<GetMotorcyclesQuery, Result<List<Domains.Entities.Motorcycle>>>
    {
        private readonly IMotorcycleRepository _repository;

        public GetMotorcyclesHandler(IMotorcycleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<Domains.Entities.Motorcycle>>> Handle(GetMotorcyclesQuery request, CancellationToken cancellationToken)
        {
            List<Domains.Entities.Motorcycle> motorcycles;

            if (!string.IsNullOrEmpty(request.LicensePlate))
                motorcycles = await _repository.GetByLicensePlateFilterAsync(request.LicensePlate);
            else
                motorcycles = await _repository.GetAllAsync();

            return Result<List<Domains.Entities.Motorcycle>>.Success(motorcycles, "Motorcycles retrieved successfully");
        }
    }
}