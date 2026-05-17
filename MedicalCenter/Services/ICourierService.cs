using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface ICourierService
    {
        public Task<List<CourierDto>> GetAllCourierAsync();
        public Task<CourierDto> GetCourierByIdAsync(Guid id);
        public Task<CourierDto> GetCourierByUserIdAsync(Guid userId);
        public Task CreateCourierAsync(AdminCreateDto dto);
        public Task DeleteCourierAsync(Guid CourierId);
        public Task<UpdateProfileDto> GetCourierProfileAsync(Guid id);
        public Task UpdateCourierProfileAsync(Guid id, UpdateProfileDto dto);
    }
}
