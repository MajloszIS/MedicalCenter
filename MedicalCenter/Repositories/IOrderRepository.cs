using MedicalCenter.Models;


namespace MedicalCenter.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrdersByPatientIdAsync(Guid patientId);
        Task<List<Order>> GetAllOrdersAsync(int skip, int take);
        public Task<int> GetOrdersCountAsync();
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<Invoice> GetInvoiceByOrderIdAsync(Guid orderId);
        Task AddOrderRatingAsync(OrderRating rating);
        Task<Dictionary<Guid, OrderRating>> GetOrderRatingsMapAsync();
    }
}