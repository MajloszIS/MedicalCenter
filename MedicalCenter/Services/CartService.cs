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

        public async Task CreateOrderFromCartAsync(Guid patientId, string sessionId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(patientId);

            if (cart == null || !cart.Items.Any()) return;

            var orderItems = cart.Items.Select(ci => new OrderItem
            {
                MedicineId = ci.MedicineId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Medicine.Price
            }).ToList();

            var order = new Order
            {
                PatientId = patientId,
                StatusId = 1,
                StripeSessionId = sessionId,
                TotalPrice = orderItems.Sum(i => i.Quantity * i.UnitPrice),
                Items = orderItems
            };

            await _cartRepository.AddOrderAsync(order);
            _cartRepository.RemoveCartItems(cart.Items);

            await _cartRepository.SaveChangesAsync();
        }

        public async Task ConfirmPaymentAsync(string sessionId)
        {
            var order = await _cartRepository.GetOrderBySessionIdAsync(sessionId);

            if (order != null)
            {
                order.StatusId = 2;

                var delivery = new Delivery
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    CourierId = null,
                    StatusId = 1
                };

                await _cartRepository.AddDeliveryAsync(delivery);
                await _cartRepository.SaveChangesAsync();
            }
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