using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Motorcycle.Commands;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Handlers
{
    public class DeleteMotorcycleHandler : IRequestHandler<DeleteMotorcycleCommand, Result>
    {
        private readonly IMotorcycleRepository _repository;

        public DeleteMotorcycleHandler(IMotorcycleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(DeleteMotorcycleCommand request, CancellationToken cancellationToken)
        {
            var motorcycle = await _repository.GetByIdAsync(request.Id);
            if (motorcycle == null)
            {
                return Result.Failure("Motorcycle not found");
            }

            var hasRentals = await _repository.HasRentalsAsync(request.Id);
            if (hasRentals)
            {
                return Result.Failure("Cannot delete motorcycle with existing rentals");
            }

            await _repository.DeleteAsync(request.Id);

            return Result.Success("Motorcycle deleted successfully");
        }
    }
}