using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IPatientService
    {
        public Task<List<PatientDto>> GetAllPatientsAsync();
        public Task RegisterAsync(PatientRegisterDto dto);
        public Task RegisterGoogleUserAsync(string email, string firstName, string lastName);
        public Task<PatientDto> GetUserByEmailAsync(string email);
        public Task<PatientDto> GetPatientByIdAsync(Guid id);
        public Task<PatientDto> GetPatientByUserIdAsync(Guid userId);
        public Task<UpdatePatientProfileDto> GetPatientProfileAsync(Guid id);
        public Task UpdatePatientProfileAsync(Guid id, UpdatePatientProfileDto dto);
        public Task DeletePatientAsync(Guid patientId);
    }
}
