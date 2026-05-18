using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrdersByPatientIdAsync(Guid patientId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(Guid orderId);
    }
}