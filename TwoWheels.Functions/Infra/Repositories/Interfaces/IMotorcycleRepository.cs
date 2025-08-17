using TwoWheels.Functions.Domains.Entities;

namespace TwoWheels.Functions.Infra.Repositories.Interfaces
{
    public interface IMotorcycleRepository
    {
        Task<Motorcycle?> GetByIdAsync(string id);
        Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate);
        Task<List<Motorcycle>> GetByLicensePlateFilterAsync(string licensePlate);
        Task<List<Motorcycle>> GetAllAsync();
        Task CreateAsync(Motorcycle motorcycle);
        Task UpdateAsync(Motorcycle motorcycle);
        Task DeleteAsync(string id);
        Task<bool> HasRentalsAsync(string motorcycleId);
    }
}
