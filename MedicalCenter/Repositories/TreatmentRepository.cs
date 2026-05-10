using MedicalCenter.Models;
using MedicalCenter.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class TreatmentRepository : ITreatmentRepository
    {
        private readonly AppDbContext _context;
        public TreatmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddTreatmentAsync(Treatment treatment)
        {
            _context.Treatments.Add(treatment);
            await _context.SaveChangesAsync();
        }
        public async Task<Treatment> GetTreatmentByIdAsync(Guid id)
        {
            return await _context.Treatments.FindAsync(id);
        }
        public async Task<List<Treatment>> GetTreatmentsByDiagnosisIdAsync(Guid diagnosisId)
        {
            return await _context.Treatments.Where(t => t.DiagnosisId == diagnosisId).ToListAsync();
        }
        public async Task UpdateTreatmentAsync(Treatment treatment)
        {
            _context.Treatments.Update(treatment);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTreatmentAsync(Guid treatmentId)
        {
            var treatment = await _context.Treatments.FirstOrDefaultAsync(t => t.Id == treatmentId);
            if (treatment == null)
            {
                throw new Exception("Treatment not found");
            }
            _context.Treatments.Remove(treatment);
            await _context.SaveChangesAsync();
        }
    }
}
