using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> GetCartAsync(Guid patientId)
        {
            return await _cartRepository.GetCartWithItemsAsync(patientId);
        }

        public async Task AddToCartAsync(Guid patientId, Guid medicineId, int quantity)
        {
            var cart = await _cartRepository.GetCartAsync(patientId);

            if (cart == null)
            {
                cart = new Cart { PatientId = patientId };
                await _cartRepository.AddCartAsync(cart);
                await _cartRepository.SaveChangesAsync();
            }

            var existingItem = await _cartRepository.GetCartItemAsync(cart.Id, medicineId);

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
                await _cartRepository.AddCartItemAsync(newItem);
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task CreateOrderFromCartAsync(Guid patientId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(patientId);

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

            await _cartRepository.AddOrderAsync(order);
            _cartRepository.RemoveCartItems(cart.Items);

            await _cartRepository.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(Guid patientId, Guid medicineId)
        {
            var cart = await _cartRepository.GetCartAsync(patientId);
            if (cart == null) return;

            var itemToRemove = await _cartRepository.GetCartItemAsync(cart.Id, medicineId);

            if (itemToRemove != null)
            {
                _cartRepository.RemoveCartItem(itemToRemove);
                await _cartRepository.SaveChangesAsync();
            }
        }
    }
}