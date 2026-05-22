using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface ICourierService
    {
        public Task<List<CourierDto>> GetAllCourierAsync();
        public Task<CourierDto> GetCourierByIdAsync(Guid id);
        public Task<CourierDto> GetCourierByUserIdAsync(Guid userId);
        public Task CreateCourierAsync(AdminCreateDto dto);
        public Task DeleteCourierAsync(Guid CourierId);
        public Task<CourierProfileDto> GetCourierProfileAsync(Guid id);
        public Task UpdateCourierProfileAsync(Guid id, UpdateCourierProfileDto dto);
        public Task ChangeDeliveryStatusAsync(Guid deliveryId, int newStatusId);
    }
}
