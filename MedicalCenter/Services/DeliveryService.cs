using MedicalCenter.DTOs;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public DeliveryService(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<List<DeliveryDto>> GetAllDeliveriesAsync()
        {
            var deliveries = await _deliveryRepository.GetAllDeliveriesAsync();

            return deliveries.Select(d => new DeliveryDto
            {
                Id = d.Id,
                OrderId = d.OrderId,
                PatientFullName = d.Order.Patient.User.FirstName + " " + d.Order.Patient.User.LastName,
                TotalPrice = d.Order.TotalPrice,
                StatusName = d.Status != null ? d.Status.Name : "Brak statusu",
                DeliveryAddress = "Do uzupełnienia z Address",
                City = "Warszawa",
                PostalCode = "00-000"
            }).ToList();
        }

        public async Task ChangeStatusAsync(Guid deliveryId, string statusName)
        {
            var delivery = await _deliveryRepository.GetDeliveryByIdAsync(deliveryId);
            if (delivery == null) return;

            var newStatus = await _deliveryRepository.GetStatusByNameAsync(statusName);
            if (newStatus != null)
            {
                delivery.StatusId = newStatus.Id;
                await _deliveryRepository.UpdateDeliveryAsync(delivery);
            }
        }
    }
}