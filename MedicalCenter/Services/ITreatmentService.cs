using MedicalCenter.Models;
using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface ITreatmentService
    {
        public Task CreateTreatmentAsync(TreatmentDto treatmentDto);
        public Task<TreatmentDto> GetTreatmentByIdAsync(Guid id);
        public Task<List<TreatmentDto>> GetTreatmentsByDiagnosisIdAsync(Guid diagnosisId);
        public Task UpdateTreatmentAsync(TreatmentDto treatmentDto);
        public Task DeleteTreatmentAsync(Guid treatmentId);
    }
}
