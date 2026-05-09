using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IDoctorService
    {
        public Task<List<DoctorDto>> GetAllDoctorsAsync();
        public Task<DoctorDto> GetDoctorByIdAsync(Guid id);
        public Task <DoctorDto> GetDoctorByUserIdAsync(Guid userId);
        public Task CreateDoctorAsync(AdminCreateDoctorDto dto);
        public Task DeleteDoctorAsync(Guid DoctorId);
        public Task<UpdateDoctorProfileDto> GetDoctorProfileAsync(Guid id);
        public Task UpdateDoctorProfileAsync(Guid id, UpdateDoctorProfileDto dto);
    }
}
