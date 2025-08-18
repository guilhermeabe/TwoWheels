using MediatR;
using TwoWheels.Functions.Domains.Entities;
using TwoWheels.Functions.Domains.Enuns;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Rental.Commands;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Rental.Handlers
{
    public class CreateRentalHandler : IRequestHandler<CreateRentalCommand, Result<string>>
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IDelivererRepository _delivererRepository;
        private readonly IMotorcycleRepository _motorcycleRepository;

        public CreateRentalHandler(
            IRentalRepository rentalRepository,
            IDelivererRepository delivererRepository,
            IMotorcycleRepository motorcycleRepository)
        {
            _rentalRepository = rentalRepository;
            _delivererRepository = delivererRepository;
            _motorcycleRepository = motorcycleRepository;
        }

        public async Task<Result<string>> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
        {
            var deliverer = await _delivererRepository.GetByIdAsync(request.DelivererId);
            if (deliverer == null)
            {
                return Result<string>.Failure("Deliverer not found");
            }

            if (deliverer.CnhType != CnhType.A && deliverer.CnhType != CnhType.AB)
            {
                return Result<string>.Failure("Only deliverers with CNH type A or AB can rent motorcycles");
            }

            var motorcycle = await _motorcycleRepository.GetByIdAsync(request.MotorcycleId);
            if (motorcycle == null)
            {
                return Result<string>.Failure("Motorcycle not found");
            }

            var hasActiveRental = await _rentalRepository.HasActiveRentalForDelivererAsync(request.DelivererId);
            if (hasActiveRental)
            {
                return Result<string>.Failure("Deliverer already has an active rental");
            }

            var motorcycleHasActiveRental = await _rentalRepository.HasActiveRentalForMotorcycleAsync(request.MotorcycleId);
            if (motorcycleHasActiveRental)
            {
                return Result<string>.Failure("Motorcycle is already rented");
            }

            var plan = Array.Find(RentalPlan.AvailablePlans, p => p.Days == request.PlanDays);
            if (plan == null)
            {
                return Result<string>.Failure("Invalid rental plan");
            }

            var expectedStartDate = DateTime.UtcNow.Date.AddDays(1);
            if (request.StartDate.Date != expectedStartDate)
            {
                return Result<string>.Failure($"Start date must be {expectedStartDate:yyyy-MM-dd}");
            }

            var rental = new Domains.Entities.Rental
            {
                Id = Guid.NewGuid().ToString(),
                DelivererId = deliverer.Id,
                MotorcycleId = motorcycle.Id,
                Plan = plan,
                StartDate = request.StartDate,
                ExpectedEndDate = request.ExpectedEndDate,
                TotalAmount = plan.Days * plan.DailyRate,
                CreatedAt = DateTime.UtcNow
            };

            await _rentalRepository.CreateAsync(rental);

            return Result<string>.Success(rental.Id, "Rental created successfully");
        }
    }
}