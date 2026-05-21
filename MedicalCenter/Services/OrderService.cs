using MedicalCenter.DTOs;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderDto>> GetPatientOrdersAsync(Guid patientId)
        {
            var orders = await _orderRepository.GetOrdersByPatientIdAsync(patientId);
            return MapToDto(orders);
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return MapToDto(orders);
        }

        private List<OrderDto> MapToDto(List<MedicalCenter.Models.Order> orders)
        {
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,

                StatusName = o.Status?.Name ?? TranslateStatusId(o.StatusId.ToString()),

                PatientFullName = o.Patient?.User != null ? $"{o.Patient.User.FirstName} {o.Patient.User.LastName}" : "Nieznany pacjent",
                Items = o.Items?.Select(i => new OrderItemDto
                {
                    MedicineName = i.Medicine?.Name ?? "Nieznany lek",
                    Quantity = i.Quantity
                }).ToList() ?? new List<OrderItemDto>()
            }).ToList();
        }

        private string TranslateStatusId(string statusIdStr)
        {
            if (statusIdStr == "1" || statusIdStr.Contains("11111111-1111")) return "Nowe";
            if (statusIdStr == "2" || statusIdStr.Contains("22222222-2222")) return "W realizacji";
            if (statusIdStr == "3" || statusIdStr.Contains("33333333-3333")) return "Wysłane";
            if (statusIdStr == "4" || statusIdStr.Contains("44444444-4444")) return "Zakończone";

            return "Weryfikacja płatności"; 
        }
        public async Task<InvoiceDto?> GetInvoiceByOrderIdAsync(Guid orderId)
        {
            var invoice = await _orderRepository.GetInvoiceByOrderIdAsync(orderId);
            if (invoice == null) return null;

            var address = invoice.Patient?.Address;
            string? addressLine1 = address != null
                ? $"{address.Street} {address.HouseNumber}" + (!string.IsNullOrEmpty(address.ApartmentNumber) ? $"/{address.ApartmentNumber}" : "")
                : null;
            string? addressLine2 = address != null
                ? $"{address.PostalCode} {address.City}"
                : null;

            return new InvoiceDto
            {
                Id = invoice.Id,
                OrderId = invoice.OrderId,
                IssuedAt = invoice.IssuedAt,
                PatientUserId = invoice.Patient?.UserId ?? Guid.Empty,
                PatientFullName = invoice.Patient?.User != null
                    ? $"{invoice.Patient.User.FirstName} {invoice.Patient.User.LastName}"
                    : "Nieznany pacjent",
                PatientIdStr = invoice.PatientId.ToString().Substring(0, 8),

                PatientAddressLine1 = addressLine1,
                PatientAddressLine2 = addressLine2,

                Amount = invoice.Amount,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                StripePaymentId = invoice.StripePaymentId,
                Items = invoice.Order?.Items?.Select(oi => new InvoiceItemDto
                {
                    MedicineName = oi.Medicine?.Name ?? "Nieznany lek",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<InvoiceItemDto>()
            };
        }
    }
}