using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IDeliveryService
    {
        Task<List<DeliveryDto>> GetAllDeliveriesAsync();
        Task ChangeStatusAsync(Guid deliveryId, string statusName);
    }
}