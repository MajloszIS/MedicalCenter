using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IDeliveryService
    {
        Task<List<DeliveryDto>> GetAllDeliveriesAsync();
        Task ChangeStatusAsync(Guid deliveryId, string statusName);
        Task<List<DeliveryDto>> GetAvailableDeliveriesAsync();
        Task<List<DeliveryDto>> GetMyDeliveriesAsync(Guid courierId);
        Task AcceptDeliveryAsync(Guid deliveryId, Guid courierId);
    }
}