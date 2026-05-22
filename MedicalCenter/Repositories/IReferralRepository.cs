using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IReferralRepository
    {
        public Task AddReferralAsync(Referral referral);
        public Task<List<Referral>> GetReferralsByPatientIdAsync(Guid patientId);
        public Task<Referral?> GetMedicalLeaveIdAsync(Guid id);
    }
}
