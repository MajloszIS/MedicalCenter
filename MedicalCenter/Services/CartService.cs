using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(Guid patientId, Guid medicineId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.PatientId == patientId);

            if (cart == null)
            {
                cart = new Cart { PatientId = patientId, Items = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.MedicineId == medicineId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem { MedicineId = medicineId, Quantity = quantity });
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateOrderFromCartAsync(Guid patientId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(c => c.PatientId == patientId);

            if (cart == null || !cart.Items.Any()) return;

            var order = new Order
            {
                PatientId = patientId,
                StatusId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                TotalPrice = cart.Items.Sum(i => i.Quantity * i.Medicine.Price),
                Items = cart.Items.Select(ci => new OrderItem
                {
                    MedicineId = ci.MedicineId,
                    Quantity = ci.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
        }
    }
}