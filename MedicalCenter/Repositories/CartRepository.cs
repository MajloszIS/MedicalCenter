using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartWithItemsAsync(Guid patientId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(c => c.PatientId == patientId);
        }

        public async Task<Cart> GetCartAsync(Guid patientId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.PatientId == patientId);
        }

        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public async Task<CartItem> GetCartItemAsync(Guid cartId, Guid medicineId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.MedicineId == medicineId);
        }

        public async Task AddCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }

        public void RemoveCartItem(CartItem item)
        {
            _context.CartItems.Remove(item);
        }

        public void RemoveCartItems(IEnumerable<CartItem> items)
        {
            _context.CartItems.RemoveRange(items);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}