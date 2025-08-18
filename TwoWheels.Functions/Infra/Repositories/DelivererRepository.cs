using Microsoft.EntityFrameworkCore;
using TwoWheels.Functions.Domains.Entities;
using TwoWheels.Functions.Infra.Repositories.Data;
using TwoWheels.Functions.Infra.Repositories.Interfaces;

namespace TwoWheels.Functions.Infra.Repositories
{
    public class DelivererRepository : IDelivererRepository
    {
        private readonly AppDbContext _context;

        public DelivererRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Deliverer?> GetByIdAsync(string id)
        {
            return await _context.Deliverers
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Deliverer?> GetByCnhNumberAsync(string cnhNumber)
        {
            return await _context.Deliverers.FindAsync(cnhNumber);
        }

        public async Task<Deliverer?> GetByCnpjAsync(string cnpj)
        {
            return await _context.Deliverers
                .FirstOrDefaultAsync(d => d.Cnpj == cnpj);
        }

        public async Task<List<Deliverer>> GetAllAsync()
        {
            return await _context.Deliverers.ToListAsync();
        }

        public async Task CreateAsync(Deliverer deliverer)
        {
            _context.Deliverers.Add(deliverer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Deliverer deliverer)
        {
            deliverer.UpdatedAt = DateTime.UtcNow;
            _context.Deliverers.Update(deliverer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var deliverer = await GetByIdAsync(id);
            if (deliverer != null)
            {
                _context.Deliverers.Remove(deliverer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasActiveRentalsAsync(string delivererId)
        {
            return await _context.Rentals
                .Join(_context.Deliverers,
                      rental => rental.DelivererId, 
                      deliverer => deliverer.CnhNumber,
                      (rental, deliverer) => new { rental, deliverer })
                .AnyAsync(x => x.deliverer.Id == delivererId && x.rental.ActualEndDate == null);
        }
    }
}