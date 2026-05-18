using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(Guid patientId);
        Task AddToCartAsync(Guid patientId, Guid medicineId, int quantity);
        Task CreateOrderFromCartAsync(Guid patientId, string sessionId);
        Task RemoveFromCartAsync(Guid patientId, Guid medicineId);
        Task ConfirmPaymentAsync(string sessionId);
    }
}