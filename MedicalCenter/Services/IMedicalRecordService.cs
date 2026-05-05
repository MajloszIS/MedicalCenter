using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IMedicalRecordService
    {
        public Task<MedicalRecordDto> GetOrCreateAsync(Guid doctorId, Guid patientId);
        public Task<MedicalRecordDto> GetMedicalRecordByIdAsync(Guid id);
    }
}
