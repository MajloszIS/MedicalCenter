using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly AppDbContext _context;

        public DeliveryRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Delivery>> GetAllDeliveriesAsync()
        {
            // Przeniesione z dawnego Serwisu - samo wyciąganie z bazy
            return _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.User)
                .Include(d => d.Status)
                .ToListAsync();
        }

        public Task<Delivery> GetDeliveryByIdAsync(Guid id)
        {
            return _context.Deliveries.FirstOrDefaultAsync(d => d.Id == id);
        }

        public Task<DeliveryStatus> GetStatusByNameAsync(string statusName)
        {
            return _context.DeliveryStatuses.FirstOrDefaultAsync(s => s.Name == statusName);
        }

        public Task UpdateDeliveryAsync(Delivery delivery)
        {
            _context.Deliveries.Update(delivery);
            return _context.SaveChangesAsync();
        }
        public async Task<List<Delivery>> GetUnassignedDeliveriesAsync()
        {
            return await _context.Deliveries
                .Include(d => d.Order).ThenInclude(o => o.Patient).ThenInclude(p => p.User)
                .Include(d => d.Status)
                .Where(d => d.CourierId == null) // Wyciągamy te, które nie mają jeszcze kuriera
                .ToListAsync();
        }

        public async Task<List<Delivery>> GetDeliveriesByCourierIdAsync(Guid courierId)
        {
            return await _context.Deliveries
                .Include(d => d.Order).ThenInclude(o => o.Patient).ThenInclude(p => p.User)
                .Include(d => d.Status)
                .Where(d => d.CourierId == courierId) // Wyciągamy tylko moje
                .ToListAsync();
        }
    }
}