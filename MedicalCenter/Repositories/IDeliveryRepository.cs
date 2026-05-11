using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IDeliveryRepository
    {
        Task<List<Delivery>> GetAllDeliveriesAsync();
        Task<Delivery> GetDeliveryByIdAsync(Guid id);
        Task<DeliveryStatus> GetStatusByNameAsync(string statusName);
        Task UpdateDeliveryAsync(Delivery delivery);
    }
}