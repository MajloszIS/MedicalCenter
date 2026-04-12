using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IPatientService
    {
        public Task<List<PatientDto>> GetAllPatientsAsync();
        public Task RegisterAsync(PatientRegisterDto dto);
        public Task<PatientDto> GetPatientByUserIdAsync(Guid userId);
    }
}
