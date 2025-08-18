using MediatR;
using TwoWheels.Functions.Infra.Repositories.Interfaces;
using TwoWheels.Functions.Services.Api.Rental.Queries;
using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Rental.Handlers
{
    public class GetRentalCalculationHandler : IRequestHandler<GetRentalCalculationQuery, Result<RentalCalculationResult>>
    {
        private readonly IRentalRepository _repository;

        public GetRentalCalculationHandler(IRentalRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<RentalCalculationResult>> Handle(GetRentalCalculationQuery request, CancellationToken cancellationToken)
        {
            var rental = await _repository.GetByIdAsync(request.Id);
            if (rental == null)
            {
                return Result<RentalCalculationResult>.Failure("Rental not found");
            }

            var result = CalculateRentalDetails(request.ReturnDate, rental);

            return Result<RentalCalculationResult>.Success(result, "Calculation completed successfully");
        }

        private RentalCalculationResult CalculateRentalDetails(DateTime returnDate, Domains.Entities.Rental rental)
        {
            var plan = rental.Plan;
            var daysUsed = (int)(returnDate.Date - rental.StartDate.Date).TotalDays + 1;
            var dailyRate = plan.DailyRate;

            decimal penaltyAmount = 0;
            decimal extraAmount = 0;
            string details = "";

            if (returnDate.Date < rental.ExpectedEndDate.Date)
            {
                var unusedDays = (int)(rental.ExpectedEndDate.Date - returnDate.Date).TotalDays;
                penaltyAmount = unusedDays * plan.DailyRate * plan.EarlyReturnPenaltyPercentage;
                details = $"Early return: {unusedDays} unused days with {plan.EarlyReturnPenaltyPercentage:P0} penalty";
            }
            else if (returnDate.Date > rental.ExpectedEndDate.Date)
            {
                var lateDays = (int)(returnDate.Date - rental.ExpectedEndDate.Date).TotalDays;
                extraAmount = lateDays * plan.LateReturnDailyFee;
                details = $"Late return: {lateDays} extra days at R${plan.LateReturnDailyFee:F2} per day";
            }
            else
            {
                details = "On-time return";
            }

            var totalAmount = (daysUsed * dailyRate) + penaltyAmount + extraAmount;

            return new RentalCalculationResult
            {
                TotalAmount = totalAmount,
                DailyRate = dailyRate,
                DaysUsed = daysUsed,
                PenaltyAmount = penaltyAmount,
                ExtraAmount = extraAmount,
                CalculationDetails = details
            };
        }
    }
}