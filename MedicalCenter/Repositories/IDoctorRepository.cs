using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IDoctorRepository
    {
        public Task<List<Doctor>> GetAllDoctorsAsync();
        public Task<Doctor> GetDoctorByIdAsync(Guid id);
        public Task CreateDoctorAsync(Doctor doctor);
        public Task UpdateDoctorAsync(Doctor doctor);
        public Task DeleteDoctorAsync(Guid id);
        public Task<Doctor> GetDoctorByUserIdAsync(Guid userId);
        public Task<List<Doctor>> GetDoctorsBySpecializationAsync(string specializationName);

    }
}
