using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByPatientIdAsync(Guid patientId)
        {
            return await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.Items).ThenInclude(i => i.Medicine)
                .Where(o => o.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.Patient).ThenInclude(p => p.User)
                .Include(o => o.Items).ThenInclude(i => i.Medicine)
                .ToListAsync();
        }
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}