using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IPrescriptionService
    {
        public Task<PrescriptionDto> GetByIdAsync(Guid id);
        public Task CreatePrescription(PrescriptionDto prescriptionDto);
        public Task UpdatePrescription(PrescriptionDto prescriptionDto);
        public Task DeletePrescription(Guid id);
    }
}
