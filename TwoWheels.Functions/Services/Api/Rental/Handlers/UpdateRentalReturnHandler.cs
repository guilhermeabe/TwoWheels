using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Rental.Commands;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Rental.Handlers
{
    public class UpdateRentalReturnHandler : IRequestHandler<UpdateRentalReturnCommand, Result<string>>
    {
        private readonly IRentalRepository _repository;

        public UpdateRentalReturnHandler(IRentalRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<string>> Handle(UpdateRentalReturnCommand request, CancellationToken cancellationToken)
        {
            var rental = await _repository.GetByIdAsync(request.Id);
            if (rental == null)
                return Result<string>.Failure("Rental not found");

            if (rental.ActualEndDate.HasValue)
                return Result<string>.Failure("Rental already returned");

            var totalAmount = CalculateRentalCost(request.ReturnDate, rental);

            rental.ActualEndDate = request.ReturnDate;
            rental.TotalAmount = totalAmount;

            await _repository.UpdateAsync(rental);

            return Result<string>.Success(rental.TotalAmount.ToString(), "Return date updated successfully");
        }

        private static decimal CalculateRentalCost(DateTime returnDate, Domains.Entities.Rental rental)
        {
            var plan = rental.Plan;
            var daysUsed = (int)(returnDate.Date - rental.StartDate.Date).TotalDays;
            var baseCost = plan.Days * plan.DailyRate;

            if (returnDate.Date < rental.ExpectedEndDate.Date)
            {
                var unusedDays = (int)(rental.ExpectedEndDate.Date - returnDate.Date).TotalDays;
                var penalty = unusedDays * plan.DailyRate * plan.EarlyReturnPenaltyPercentage;
                return (daysUsed * plan.DailyRate) + penalty;
            }
            else if (returnDate.Date > rental.ExpectedEndDate.Date)
            {
                var lateDays = (int)(returnDate.Date - rental.ExpectedEndDate.Date).TotalDays;
                var lateFee = lateDays * plan.LateReturnDailyFee;
                return baseCost + lateFee;
            }
            else
            {
                return baseCost;
            }
        }
    }
}