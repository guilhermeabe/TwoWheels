using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Motorcycle.Queries;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Handlers
{
    public class GetMotorcycleByIdHandler : IRequestHandler<GetMotorcycleByIdQuery, Result<Domains.Entities.Motorcycle>>
    {
        private readonly IMotorcycleRepository _repository;

        public GetMotorcycleByIdHandler(IMotorcycleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Domains.Entities.Motorcycle>> Handle(GetMotorcycleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var motorcycle = await _repository.GetByIdAsync(request.Id);

                if (motorcycle == null)
                {
                    return Result<Domains.Entities.Motorcycle>.Failure("Motocicleta não encontrada");
                }

                return Result<Domains.Entities.Motorcycle>.Success(motorcycle);
            }
            catch (Exception ex)
            {
                return Result<Domains.Entities.Motorcycle>.Failure($"Erro ao buscar motocicleta: {ex.Message}");
            }
        }
    }
}