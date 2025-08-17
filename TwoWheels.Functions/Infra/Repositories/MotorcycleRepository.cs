using Microsoft.EntityFrameworkCore;
using TwoWheels.Functions.Domains.Entities;
using TwoWheels.Functions.Infra.Repositories.Data;
using TwoWheels.Functions.Infra.Repositories.Interfaces;

namespace TwoWheels.Functions.Infra.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly AppDbContext _context;

        public MotorcycleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Motorcycle?> GetByIdAsync(string id)
        {
            return await _context.Motorcycles.FindAsync(id);
        }

        public async Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate)
        {
            return await _context.Motorcycles
                .FirstOrDefaultAsync(m => m.LicensePlate == licensePlate);
        }

        public async Task<List<Motorcycle>> GetByLicensePlateFilterAsync(string licensePlate)
        {
            return await _context.Motorcycles
                .Where(m => m.LicensePlate.Contains(licensePlate))
                .ToListAsync();
        }

        public async Task<List<Motorcycle>> GetAllAsync()
        {
            return await _context.Motorcycles.ToListAsync();
        }

        public async Task CreateAsync(Motorcycle motorcycle)
        {
            _context.Motorcycles.Add(motorcycle);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Motorcycle motorcycle)
        {
            motorcycle.UpdatedAt = DateTime.UtcNow;
            _context.Motorcycles.Update(motorcycle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var motorcycle = await GetByIdAsync(id);
            if (motorcycle != null)
            {
                _context.Motorcycles.Remove(motorcycle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasRentalsAsync(string motorcycleId)
        {
            return await _context.Rentals.AnyAsync(r => r.MotorcycleId == motorcycleId);
        }
    }
}
