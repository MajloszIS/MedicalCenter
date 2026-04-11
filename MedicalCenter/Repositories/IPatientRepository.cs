using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IPatientRepository
    {
        public Task<List<Patient>> GetAllPatientsAsync();
        public Task<Patient> GetPatientByIdAsync(Guid id);
        public Task CreatePatientAsync(Patient patient);
        public Task UpdatePatientAsync(Patient patient);
        public Task DeletePatientAsync (Guid id);
        public Task<Patient> GetPatientByUserIdAsync(Guid userId);
    }
}
