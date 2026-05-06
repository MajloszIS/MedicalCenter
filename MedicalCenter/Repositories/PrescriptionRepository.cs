using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly AppDbContext _context;
        public PrescriptionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Prescription> GetByIdAsync(Guid id)
        {
            return await _context.Prescriptions
                .Include(p => p.Items)
                    .ThenInclude(i => i.Medicine)
                .Include(p => p.MedicalRecord)
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task CreatePrescriptionAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
            await _context.SaveChangesAsync();
        }
        public async Task UpdatePrescriptionAsync(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);
            await _context.SaveChangesAsync();
        }
        public async Task DeletePrescriptionAsync(Prescription prescription)
        {
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
        }
        public Task<List<Prescription>> GetPrescriptionsByPatientIdAsync(Guid patientId)
        {
            return _context.Prescriptions
                .Include(p => p.MedicalRecord)
                .Include(p => p.Doctor)
                .Include(p => p.Items)
                    .ThenInclude(i => i.Medicine)
                .Where(p => p.MedicalRecord.PatientId == patientId)
                .ToListAsync();
        }
    }
}
