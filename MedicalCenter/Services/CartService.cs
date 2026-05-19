using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMedicineRepository _medicineRepository;

        public CartService(ICartRepository cartRepository, IMedicineRepository medicineRepository)
        {
            _cartRepository = cartRepository;
            _medicineRepository = medicineRepository;
        }

        public async Task<Cart> GetCartAsync(Guid patientId)
        {
            return await _cartRepository.GetCartWithItemsAsync(patientId);
        }

        public async Task<(bool success, string message)> AddToCartAsync(Guid patientId, Guid medicineId, int quantity)
        {
            var medicine = await _medicineRepository.GetByIdAsync(medicineId);

            if (medicine == null || medicine.StockQuantity <= 0 || quantity <= 0)
            {
                return (false, "Lek jest niedostępny lub podano błędną ilość.");
            }

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
                if (existingItem.Quantity + quantity > medicine.StockQuantity)
                {
                    int possibleToAdd = medicine.StockQuantity - existingItem.Quantity;

                    if (possibleToAdd <= 0)
                    {
                        return (false, $"Nie możesz dodać więcej. Masz już w koszyku maksymalną dostępną ilość tego leku ({medicine.StockQuantity} szt.).");
                    }
                    else
                    {
                        existingItem.Quantity = medicine.StockQuantity;
                        await _cartRepository.SaveChangesAsync();
                        return (true, $"Dodano {possibleToAdd} szt. Osiągnięto limit dostępności w magazynie ({medicine.StockQuantity} szt.).");
                    }
                }
                else
                {
                    existingItem.Quantity += quantity;
                    await _cartRepository.SaveChangesAsync();
                    return (true, "Lek został pomyślnie dodany do koszyka!");
                }
            }
            else
            {
                if (quantity > medicine.StockQuantity)
                {
                    var newItem = new CartItem { CartId = cart.Id, MedicineId = medicineId, Quantity = medicine.StockQuantity };
                    await _cartRepository.AddCartItemAsync(newItem);
                    await _cartRepository.SaveChangesAsync();
                    return (true, $"Dodano {medicine.StockQuantity} szt. Osiągnięto limit dostępności w magazynie.");
                }
                else
                {
                    var newItem = new CartItem { CartId = cart.Id, MedicineId = medicineId, Quantity = quantity };
                    await _cartRepository.AddCartItemAsync(newItem);
                    await _cartRepository.SaveChangesAsync();
                    return (true, "Lek został pomyślnie dodany do koszyka!");
                }
            }
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