namespace TwoWheels.Functions.Services.Events.Motorcycle.Models
{
    public class MotorcycleCreatedEvent
    {
        public string Id { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string EventType { get; set; } = "MotorcycleCreated";
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public DateTime EventTimestamp { get; set; } = DateTime.UtcNow;
    }
}
