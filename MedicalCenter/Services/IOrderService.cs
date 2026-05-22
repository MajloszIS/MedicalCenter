using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetPatientOrdersAsync(Guid patientId);
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<InvoiceDto> GetInvoiceByOrderIdAsync(Guid orderId);
        Task AddOrderRatingAsync(OrderRating rating);
        byte[] GenerateInvoicePdf(InvoiceDto invoice);
    }
}