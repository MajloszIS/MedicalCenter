using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetCartWithItemsAsync(Guid patientId);
        Task<Cart> GetCartAsync(Guid patientId);
        Task AddCartAsync(Cart cart);
        Task<CartItem> GetCartItemAsync(Guid cartId, Guid medicineId);
        Task AddCartItemAsync(CartItem item);
        void RemoveCartItem(CartItem item);
        void RemoveCartItems(IEnumerable<CartItem> items);
        Task AddOrderAsync(Order order);
        Task SaveChangesAsync();
    }
}