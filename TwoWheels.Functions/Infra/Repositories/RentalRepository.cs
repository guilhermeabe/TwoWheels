using Microsoft.EntityFrameworkCore;
using TwoWheels.Functions.Domains.Entities;
using TwoWheels.Functions.Infra.Repositories.Data;
using TwoWheels.Functions.Infra.Repositories.Interfaces;

namespace TwoWheels.Functions.Infra.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly AppDbContext _context;

        public RentalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Rental?> GetByIdAsync(string id)
        {
            return await _context.Rentals
                .Include(r => r.Deliverer)
                .Include(r => r.Motorcycle)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Rental>> GetByDelivererIdAsync(string delivererId)
        {
            return await _context.Rentals
                .Include(r => r.Deliverer)
                .Include(r => r.Motorcycle)
                .Where(r => r.Deliverer != null && r.Deliverer.Id == delivererId)
                .ToListAsync();
        }

        public async Task<List<Rental>> GetByMotorcycleIdAsync(string motorcycleId)
        {
            return await _context.Rentals
                .Include(r => r.Deliverer)
                .Include(r => r.Motorcycle)
                .Where(r => r.Motorcycle != null && r.Motorcycle.Id == motorcycleId)
                .ToListAsync();
        }

        public async Task<List<Rental>> GetAllAsync()
        {
            return await _context.Rentals
                .Include(r => r.Deliverer)
                .Include(r => r.Motorcycle)
                .ToListAsync();
        }

        public async Task CreateAsync(Rental rental)
        {
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rental rental)
        {
            _context.Rentals.Update(rental);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasActiveRentalForDelivererAsync(string delivererId)
        {
            return await _context.Rentals
                .Join(_context.Deliverers,
                      rental => rental.DelivererId,
                      deliverer => deliverer.CnhNumber,
                      (rental, deliverer) => new { rental, deliverer })
                .AnyAsync(x => x.deliverer.Id == delivererId && x.rental.ActualEndDate == null);
        }

        public async Task<bool> HasActiveRentalForMotorcycleAsync(string motorcycleId)
        {
            return await _context.Rentals
                .Join(_context.Motorcycles,
                      rental => rental.MotorcycleId,
                      motorcycle => motorcycle.LicensePlate,
                      (rental, motorcycle) => new { rental, motorcycle })
                .AnyAsync(x => x.motorcycle.Id == motorcycleId && x.rental.ActualEndDate == null);
        }
    }
}