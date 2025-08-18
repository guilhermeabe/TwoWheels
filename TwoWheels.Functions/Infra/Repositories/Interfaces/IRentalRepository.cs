using TwoWheels.Functions.Domains.Entities;

namespace TwoWheels.Functions.Infra.Repositories.Interfaces
{
    public interface IRentalRepository
    {
        Task<Rental?> GetByIdAsync(string id);
        Task<List<Rental>> GetByDelivererIdAsync(string delivererId);
        Task<List<Rental>> GetByMotorcycleIdAsync(string motorcycleId);
        Task<List<Rental>> GetAllAsync();
        Task CreateAsync(Rental rental);
        Task UpdateAsync(Rental rental);
        Task DeleteAsync(string id);
        Task<bool> HasActiveRentalForDelivererAsync(string delivererId);
        Task<bool> HasActiveRentalForMotorcycleAsync(string motorcycleId);
    }
}