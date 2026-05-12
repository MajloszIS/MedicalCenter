using MedicalCenter.Data;
using MedicalCenter.DTOs;
using Microsoft.EntityFrameworkCore;
using MedicalCenter.Repositories;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public DeliveryService(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
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

            if (delivery != null && delivery.CourierId == null)
            {
                delivery.CourierId = courierId; // Przypisanie do kuriera

                var inProgressStatus = await _deliveryRepository.GetStatusByNameAsync("W drodze");
                if (inProgressStatus != null)
                {
                    delivery.StatusId = inProgressStatus.Id; // Automatyczna zmiana statusu
                }

                await _deliveryRepository.UpdateDeliveryAsync(delivery);
            }
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

        // Oryginalna metoda do wyświetlania wszystkiego
        public async Task<List<DeliveryDto>> GetAllDeliveriesAsync()
        {
            var deliveries = await _deliveryRepository.GetAllDeliveriesAsync();
            return MapToDtoList(deliveries);
        }

        // Pomocnicza metoda do tworzenia DTO, żeby nie kopiować kodu
        private List<DeliveryDto> MapToDtoList(List<Delivery> deliveries)
        {
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
    }
}