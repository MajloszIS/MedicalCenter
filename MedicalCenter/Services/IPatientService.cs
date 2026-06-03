using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IPatientService
    {
        public Task<List<PatientDto>> GetAllPatientsAsync(int skip, int take);
        public Task<int> GetPatientsCountAsync();
        public Task RegisterAsync(PatientRegisterDto dto);
        public Task RegisterGoogleUserAsync(string email, string firstName, string lastName);
        public Task<PatientDto> GetPatientByIdAsync(Guid id);
        public Task<PatientDto> GetPatientByUserIdAsync(Guid userId);
        public Task<PatientProfileDto> GetPatientProfileAsync(Guid id);
        public Task UpdatePatientProfileAsync(Guid id, UpdatePatientProfileDto dto);
        public Task DeletePatientAsync(Guid patientId);
        public Task<List<PatientDemographicsDto>> GetPatientDemographicsAsync(int ageFrom, int ageTo);

    }
}
