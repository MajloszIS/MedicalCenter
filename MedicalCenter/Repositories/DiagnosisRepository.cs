using MedicalCenter.DTOs;
using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class DiagnosisRepository : IDiagnosisRepository
    {
        private readonly AppDbContext _context;
        public DiagnosisRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Diagnosis> GetByIdAsync(Guid id)
        {
            return await _context.Diagnoses
                .Include(d => d.Treatments)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task CreateDiagnosisAsync(Diagnosis diagnosis)
        {
            await _context.Diagnoses.AddAsync(diagnosis);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateDiagnosisAsync(Diagnosis diagnosis)
        {
            _context.Diagnoses.Update(diagnosis);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteDiagnosisAsync(Guid id)
        {
            var diagnosis = await _context.Diagnoses.Include(d => d.Treatments).FirstOrDefaultAsync(d => d.Id == id);
            if (diagnosis != null)
            {
                _context.Treatments.RemoveRange(diagnosis.Treatments);
                _context.Diagnoses.Remove(diagnosis);
                await _context.SaveChangesAsync();
            }
        }
    }
}
