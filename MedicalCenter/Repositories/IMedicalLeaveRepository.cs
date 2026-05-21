using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IMedicalLeaveRepository
    {
        public Task AddMedicalLeaveAsync(MedicalLeave medicalLeave);
        public Task<List<MedicalLeave>> GetMedicalLeavesByPatientIdAsync(Guid patientId);
        public Task<MedicalLeave?> GetMedicalLeaveIdAsync(Guid id);
    }
}
