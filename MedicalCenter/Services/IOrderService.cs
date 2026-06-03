using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetPatientOrdersAsync(Guid patientId);
        Task<List<OrderDto>> GetAllOrdersAsync(int skip = 0, int take = int.MaxValue);
        public Task<int> GetOrdersCountAsync();
        Task<InvoiceDto?> GetInvoiceByOrderIdAsync(Guid orderId);
        Task AddOrderRatingAsync(OrderRating rating);
        byte[] GenerateInvoicePdf(InvoiceDto invoice);
    }
}