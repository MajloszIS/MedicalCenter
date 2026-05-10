using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface IPrescriptionService
    {
        public Task<PrescriptionDto> GetByIdAsync(Guid id);
        public Task CreatePrescription(PrescriptionDto prescriptionDto);
        public Task UpdatePrescription(PrescriptionDto prescriptionDto);
        public Task DeletePrescription(Guid id);
        public Task DeletePrescriptionItemAsync(Guid prescriptionItemId);
        public Task<List<PrescriptionDto>> GetPrescriptionsByPatientIdAsync(Guid patientId);
    }
}
