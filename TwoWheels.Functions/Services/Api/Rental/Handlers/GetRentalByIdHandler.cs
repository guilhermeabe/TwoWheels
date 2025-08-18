using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Rental.Queries;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Rental.Handlers
{
    public class GetRentalByIdHandler : IRequestHandler<GetRentalByIdQuery, Result<Domains.Entities.Rental>>
    {
        private readonly IRentalRepository _repository;

        public GetRentalByIdHandler(IRentalRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Domains.Entities.Rental>> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
        {
            var rental = await _repository.GetByIdAsync(request.Id);

            if (rental == null)
            {
                return Result<Domains.Entities.Rental>.Failure("Rental not found");
            }

            return Result<Domains.Entities.Rental>.Success(rental, "Rental retrieved successfully");
        }
    }
}