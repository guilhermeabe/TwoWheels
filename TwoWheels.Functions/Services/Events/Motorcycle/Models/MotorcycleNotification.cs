using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoWheels.Functions.Services.Events.Motorcycle.Models
{
    public class MotorcycleNotification
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("motorcycleId")]
        public string MotorcycleId { get; set; } = string.Empty;

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;

        [JsonProperty("licensePlate")]
        public string LicensePlate { get; set; } = string.Empty;

        [JsonProperty("eventType")]
        public string EventType { get; set; } = "MotorcycleCreated";

        [JsonProperty("notificationReason")]
        public string NotificationReason { get; set; } = "Motorcycle registered in 2024";

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("processedAt")]
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        [JsonProperty("partitionKey")]
        public string PartitionKey => "2024";
    }

}
