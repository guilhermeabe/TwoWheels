using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Motorcycle.Commands;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Handlers
{
    public class UpdateMotorcycleHandler : IRequestHandler<UpdateMotorcycleCommand, Result>
    {
        private readonly IMotorcycleRepository _repository;

        public UpdateMotorcycleHandler(IMotorcycleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(UpdateMotorcycleCommand request, CancellationToken cancellationToken)
        {
            var motorcycle = await _repository.GetByIdAsync(request.Id);
            if (motorcycle == null)
            {
                return Result.Failure("Motorcycle not found");
            }

            var existingMotorcycleWithPlate = await _repository.GetByLicensePlateAsync(request.LicensePlate);
            if (existingMotorcycleWithPlate != null && existingMotorcycleWithPlate.Id != request.Id)
            {
                return Result.Failure("License plate already exists");
            }

            motorcycle.LicensePlate = request.LicensePlate;
            motorcycle.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(motorcycle);

            return Result.Success("Motorcycle updated successfully");
        }
    }
}