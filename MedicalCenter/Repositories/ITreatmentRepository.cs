using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface ITreatmentRepository
    {
        public Task AddTreatmentAsync(Treatment treatment);
        public Task<Treatment> GetTreatmentByIdAsync(Guid id);
        public Task<List<Treatment>> GetTreatmentsByDiagnosisIdAsync(Guid diagnosisId);
        public Task UpdateTreatmentAsync(Treatment treatment);
        public Task DeleteTreatmentAsync(Guid treatmentId);
    }
}
