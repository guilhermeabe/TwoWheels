using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Motorcycle.Commands;
using TwoWheels.Functions.Services.Events.Interfaces;
using TwoWheels.Functions.Services.Events.Motorcycle.Models;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Motorcycle.Handlers
{
    public class CreateMotorcycleHandler : IRequestHandler<CreateMotorcycleCommand, Result<string>>
    {
        private readonly IMotorcycleRepository _repository;
        private readonly IEventPublisher _eventPublisher;

        public CreateMotorcycleHandler(IMotorcycleRepository repository, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<string>> Handle(CreateMotorcycleCommand request, CancellationToken cancellationToken)
        {
            var existingMotorcycle = await _repository.GetByLicensePlateAsync(request.LicensePlate);
            if (existingMotorcycle != null)
            {
                return Result<string>.Failure("License plate already exists");
            }

            var motorcycle = new Domains.Entities.Motorcycle
            {
                Id = request.Id,
                Year = request.Year,
                Model = request.Model,
                LicensePlate = request.LicensePlate,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(motorcycle);


            var motorcycleEvent = new MotorcycleCreatedEvent
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year,
                Model = motorcycle.Model,
                LicensePlate = motorcycle.LicensePlate,
                CreatedAt = motorcycle.CreatedAt
            };
            await _eventPublisher.PublishMotorcycleCreatedAsync(motorcycleEvent);

            return Result<string>.Success(motorcycle.Id, "Motorcycle created successfully");
        }
    }
}