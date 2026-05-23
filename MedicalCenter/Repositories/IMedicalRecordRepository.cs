using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IMedicalRecordRepository
    {
        public Task<MedicalRecord?> GetByDoctorAndPatientAsync(Guid doctorId, Guid patientId);
        public Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid id);
        public Task CreateMedicalRecordAsync(MedicalRecord medicalRecord);
        public Task UpdateMedicalRecordAsync(MedicalRecord medicalRecord);
        public Task DeleteByIdAsync(Guid id);
    }
}
