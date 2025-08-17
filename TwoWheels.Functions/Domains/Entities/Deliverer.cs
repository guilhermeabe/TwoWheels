using TwoWheels.Functions.Domains.Enuns;

namespace TwoWheels.Functions.Domains.Entities
{
    public class Deliverer
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string CnhNumber { get; set; } = string.Empty;
        public CnhType CnhType { get; set; }
        public string? CnhImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
