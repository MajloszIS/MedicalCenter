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
        public Task<DoctorProfileDto> GetDoctorProfileAsync(Guid id);
        public Task UpdateDoctorProfileAsync(Guid id, UpdateDoctorProfileDto dto);
        public Task<List<SpecializationDto>> GetAllSpecializationsAsync();
        public Task<List<DoctorDto>> GetDoctorsBySpecializationAsync(string specializationName);
        public Task<DoctorWorkloadDto?> GetDoctorWorkloadAsync(Guid doctorId, DateTime dateFrom, DateTime dateTo);

    }
}
