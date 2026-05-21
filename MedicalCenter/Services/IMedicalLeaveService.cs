using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IMedicalLeaveService
    {
        public Task CreateMedicalLeaveAsync(MedicalLeaveDto medicalLeaveDto);
        public Task<List<MedicalLeaveDto>> GetMedicalLeavesByPatientIdAsync(Guid patientId);
        public Task<byte[]> GenerateMedicalLeavePdfAsync(Guid prescriptionId);

    }
}
