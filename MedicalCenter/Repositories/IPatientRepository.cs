using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IPatientRepository
    {
        public Task<List<Patient>> GetAllPatientsAsync(int skip, int take);
        public Task<int> GetPatientsCountAsync();
        public Task<Patient?> GetPatientByIdAsync(Guid id);
        public Task CreatePatientAsync(Patient patient);
        public Task UpdatePatientAsync(Patient patient);
        public Task DeletePatientAsync (Guid id);
        public Task<Patient?> GetPatientByUserIdAsync(Guid userId);
        public Task<List<PatientDemographicsDto>> GetPatientDemographicsAsync(int ageFrom, int ageTo);

    }
}
