namespace TwoWheels.Functions.Domains.Entities
{
    public class Rental
    {
        public string Id { get; set; } = string.Empty;
        public string DelivererId { get; set; } = string.Empty;
        public string MotorcycleId { get; set; } = string.Empty;
        public RentalPlan Plan { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Deliverer? Deliverer { get; set; }
        public Motorcycle? Motorcycle { get; set; }
    }
}
