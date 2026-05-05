using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IDiagnosisService
    {
        public Task<DiagnosisDto> GetByIdAsync(Guid id);
        public Task CreateDiagnosisAsync(DiagnosisDto diagnosisDto);
        public Task UpdateDiagnosisAsync(DiagnosisDto diagnosisDto);
        public Task DeleteDiagnosisAsync(Guid id);
    }
}
