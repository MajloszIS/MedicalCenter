using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class MedicalLeaveRepository : IMedicalLeaveRepository
    {
        private readonly AppDbContext _context;
        public MedicalLeaveRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddMedicalLeaveAsync(MedicalLeave medicalLeave)
        {
            await _context.MedicalLeaves.AddAsync(medicalLeave);
            await _context.SaveChangesAsync();   
        }
        public async Task<List<MedicalLeave>> GetMedicalLeavesByPatientIdAsync(Guid patientId)
        {
            var medicalLeaves = await _context.MedicalLeaves
                .Include(ml => ml.Doctor)
                    .ThenInclude(d => d.User)
                .Where(ml => ml.PatientId == patientId)
                .ToListAsync();

            return medicalLeaves;
        }
        public async Task<MedicalLeave?> GetMedicalLeaveIdAsync(Guid id)
        {
            var medicalLeave = await _context.MedicalLeaves
                .Include(ml => ml.Doctor)
                    .ThenInclude(d => d.User)
                .Include(ml => ml.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(ml => ml.Id == id);

            return medicalLeave;
        }

    }
}
