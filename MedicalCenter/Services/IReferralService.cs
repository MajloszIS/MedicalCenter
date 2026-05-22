using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface IReferralService
    {
        public Task CreateReferralAsync(ReferralDto referralDto);
        public Task<List<ReferralDto>> GetReferralsByPatientIdAsync(Guid patientId);
        public Task<byte[]> GenerateReferralPdfAsync(Guid id);

    }
}
