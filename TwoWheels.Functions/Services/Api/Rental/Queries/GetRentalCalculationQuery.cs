using TwoWheels.Functions.Shared.Mediator;

namespace TwoWheels.Functions.Services.Api.Rental.Queries
{
    public class GetRentalCalculationQuery : IQuery<RentalCalculationResult>
    {
        public string Id { get; set; } = string.Empty;
        public DateTime ReturnDate { get; set; }
    }

    public class RentalCalculationResult
    {
        public decimal TotalAmount { get; set; }
        public decimal DailyRate { get; set; }
        public int DaysUsed { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal ExtraAmount { get; set; }
        public string CalculationDetails { get; set; } = string.Empty;
    }
}