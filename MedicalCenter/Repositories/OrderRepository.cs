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
                .Include(o => o.Items)
                .ThenInclude(i => i.Medicine)
                .Where(o => o.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync(int skip = 0, int take = int.MaxValue)
        {
            return await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.Patient).ThenInclude(p => p.User)
                .Include(o => o.Items).ThenInclude(i => i.Medicine)
                .OrderByDescending(o => o.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        public async Task<int> GetOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task<Invoice> GetInvoiceByOrderIdAsync(Guid orderId)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(oi => oi.Medicine)
                .Include(i => i.Patient)
                    .ThenInclude(p => p.User)
                .Include(i => i.Patient)
                    .ThenInclude(p => p.Address)
                .FirstOrDefaultAsync(i => i.OrderId == orderId);
        }
        public async Task AddOrderRatingAsync(OrderRating rating)
        {
            await _context.OrderRatings.AddAsync(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<Guid, OrderRating>> GetOrderRatingsMapAsync()
        {
            return await _context.OrderRatings.ToDictionaryAsync(r => r.OrderId);
        }
    }
}