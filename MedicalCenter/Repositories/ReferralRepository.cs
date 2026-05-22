using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class ReferralRepository : IReferralRepository
    {
        private readonly AppDbContext _context;
        public ReferralRepository(AppDbContext context) 
        {
            _context = context;
        } 
        public async Task AddReferralAsync(Referral referral)
        {
            await _context.Referral.AddAsync(referral);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Referral>> GetReferralsByPatientIdAsync(Guid patientId)
        {
            var referrals = await _context.Referral
                .Include(ml => ml.Doctor)
                    .ThenInclude(d => d.User)
                .Where(ml => ml.PatientId == patientId)
                .ToListAsync();

            return referrals;
        }
        public async Task<Referral?> GetMedicalLeaveIdAsync(Guid id)
        {
            var referrals = await _context.Referral
                .Include(ml => ml.Doctor)
                    .ThenInclude(d => d.User)
                .Include(ml => ml.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(ml => ml.Id == id);

            return referrals;
        }
    }
}
