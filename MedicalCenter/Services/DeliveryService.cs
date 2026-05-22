using MedicalCenter.DTOs;
using MedicalCenter.Repositories;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IOrderRepository _orderRepository;

        public DeliveryService(IDeliveryRepository deliveryRepository, IOrderRepository orderRepository)
        {
            _deliveryRepository = deliveryRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<DeliveryDto>> GetAvailableDeliveriesAsync()
        {
            var deliveries = await _deliveryRepository.GetUnassignedDeliveriesAsync();
            return MapToDtoList(deliveries);
        }

        public async Task<List<DeliveryDto>> GetMyDeliveriesAsync(Guid courierId)
        {
            var deliveries = await _deliveryRepository.GetDeliveriesByCourierIdAsync(courierId);
            return MapToDtoList(deliveries);
        }

        public async Task AcceptDeliveryAsync(Guid deliveryId, Guid courierId)
        {
            var delivery = await _deliveryRepository.GetDeliveryByIdAsync(deliveryId);

            if (delivery == null)
            {
                throw new Exception("Dostawa nie istnieje lub została usunięta.");
            }

            if (delivery.CourierId != null)
            {
                throw new Exception("Ktoś był szybszy! Ta paczka została już przypisana do innego kuriera.");
            }

            delivery.CourierId = courierId;

            var inProgressStatus = await _deliveryRepository.GetStatusByNameAsync("W drodze");
            if (inProgressStatus != null)
            {
                delivery.StatusId = inProgressStatus.Id;

                var order = await _orderRepository.GetOrderByIdAsync(delivery.OrderId);
                if (order != null)
                {
                    order.StatusId = 3;
                }
            }

            await _deliveryRepository.UpdateDeliveryAsync(delivery);
        }

        public async Task ChangeStatusAsync(Guid deliveryId, string statusName)
        {
            var delivery = await _deliveryRepository.GetDeliveryByIdAsync(deliveryId);
            if (delivery == null) return;

            var newStatus = await _deliveryRepository.GetStatusByNameAsync(statusName);
            if (newStatus != null)
            {
                delivery.StatusId = newStatus.Id;

                var order = await _orderRepository.GetOrderByIdAsync(delivery.OrderId);
                if (order != null)
                {
                    if (statusName == "W drodze") order.StatusId = 3;
                    else if (statusName == "Dostarczono" || statusName == "Zakończone") order.StatusId = 4;
                }

                if (statusName == "Dostarczono" || statusName == "Zakończone")
                {
                    delivery.DeliveredAt = DateTime.Now;
                }

                await _deliveryRepository.UpdateDeliveryAsync(delivery);
            }
        }

        public async Task<List<DeliveryDto>> GetAllDeliveriesAsync()
        {
            var deliveries = await _deliveryRepository.GetAllDeliveriesAsync();
            return MapToDtoList(deliveries);
        }

        private List<DeliveryDto> MapToDtoList(List<Delivery> deliveries)
        {
            return deliveries.Select(d => new DeliveryDto
            {
                Id = d.Id,
                OrderId = d.OrderId,
                PatientFullName = d.Order.Patient.User.FirstName + " " + d.Order.Patient.User.LastName,
                TotalPrice = d.Order.TotalPrice,
                StatusName = d.Status != null ? d.Status.Name : "Brak statusu",
                DeliveryAddress = $"{d.Order.Patient.Address.Street ?? "Brak ulicy!"}, {d.Order.Patient.Address.HouseNumber}, {d.Order.Patient.Address.ApartmentNumber}",   
                City = d.Order.Patient.Address.City ?? "Brak miasta!",
                PostalCode = d.Order.Patient.Address.PostalCode ?? "Brak kodu pocztowego!"
            }).ToList();
        }
    }
}