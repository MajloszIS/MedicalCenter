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
        public async Task<Cart> GetCartAsync(Guid patientId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(c => c.PatientId == patientId);
        }

        public async Task AddToCartAsync(Guid patientId, Guid medicineId, int quantity)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.PatientId == patientId);

            if (cart == null)
            {
                cart = new Cart { PatientId = patientId };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.MedicineId == medicineId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    MedicineId = medicineId,
                    Quantity = quantity
                };
                await _context.CartItems.AddAsync(newItem);
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
                StatusId = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111"),
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