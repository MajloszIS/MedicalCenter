using MedicalCenter.Data;
using MedicalCenter.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly AppDbContext _context;

        public DeliveryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeliveryDto>> GetAllDeliveriesAsync()
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.User)
                .Include(d => d.Status)
                .Select(d => new DeliveryDto
                {
                    Id = d.Id,
                    OrderId = d.OrderId,
                    PatientFullName = d.Order.Patient.User.FirstName + " " + d.Order.Patient.User.LastName,
                    TotalPrice = d.Order.TotalPrice,
                    StatusName = d.Status != null ? d.Status.Name : "Brak statusu",
                    DeliveryAddress = "Do uzupełnienia z Address",
                    City = "Warszawa",
                    PostalCode = "00-000"
                })
                .ToListAsync();
        }

        public async Task ChangeStatusAsync(Guid deliveryId, string statusName)
        {
            var delivery = await _context.Deliveries.FirstOrDefaultAsync(d => d.Id == deliveryId);
            if (delivery == null) return;

            var newStatus = await _context.DeliveryStatuses.FirstOrDefaultAsync(s => s.Name == statusName);
            if (newStatus != null)
            {
                delivery.StatusId = newStatus.Id;
                await _context.SaveChangesAsync();
            }
        }
    }
}