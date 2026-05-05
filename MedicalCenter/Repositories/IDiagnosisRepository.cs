using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IDiagnosisRepository
    {
        public Task<Diagnosis> GetByIdAsync(Guid id);
        public Task CreateDiagnosisAsync(Diagnosis diagnosis);
        public Task UpdateDiagnosisAsync(Diagnosis diagnosis);
        public Task DeleteDiagnosisAsync(Guid id);
    }
}
