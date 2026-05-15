using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class CourierRepository : ICourierRepository
    {
        private readonly AppDbContext _context;
        public CourierRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Courier>> GetAllCourierAsync()
        {
            return await _context.Couriers
                .Include(c => c.User)
                .Include(c => c.Deliveries)
                .ToListAsync();
        }
        public async Task<Courier> GetCourierByIdAsync(Guid id)
        {
            return await _context.Couriers
                .Include(c => c.User)
                .Include(c => c.Deliveries)
                    .ThenInclude(d => d.Order)
                        .ThenInclude(o => o.Patient)
                .Include(c => c.Deliveries)
                    .ThenInclude(d => d.Order)
                        .ThenInclude(o => o.Status)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Courier> GetCourierByUserIdAsync(Guid userId)
        {
            return await _context.Couriers.Include(c => c.User).Include(c => c.Deliveries).FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task CreateCourierAsync(Courier courier)
        {
            await _context.Couriers.AddAsync(courier);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCourierAsync(Guid id, Courier courier)
        {
            _context.Couriers.Update(courier);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCourierAsync(Guid CourierId)
        {
            var courier = _context.Couriers.FirstOrDefault(c => c.Id == CourierId);
            if (courier != null)
            {
                _context.Couriers.Remove(courier);
                await _context.SaveChangesAsync();
            }

        }
    }
}
