using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetPatientOrdersAsync(Guid patientId);
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<InvoiceDto> GetInvoiceByOrderIdAsync(Guid orderId);
    }
}