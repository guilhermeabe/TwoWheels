using TwoWheels.Functions.Domains.Entities;

namespace TwoWheels.Functions.Infra.Repositories.Interfaces
{
    public interface IDelivererRepository
    {
        Task<Deliverer?> GetByIdAsync(string id);
        Task<Deliverer?> GetByCnhNumberAsync(string cnhNumber);
        Task<Deliverer?> GetByCnpjAsync(string cnpj);
        Task<List<Deliverer>> GetAllAsync();
        Task CreateAsync(Deliverer deliverer);
        Task UpdateAsync(Deliverer deliverer);
        Task DeleteAsync(string id);
        Task<bool> HasActiveRentalsAsync(string delivererId);
    }
}