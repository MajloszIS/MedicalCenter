using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly AppDbContext _context;
        public MedicalRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MedicalRecord> GetByDoctorAndPatientAsync(Guid doctorId, Guid patientId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(mr => mr.DoctorId == doctorId && mr.PatientId == patientId);
        }
        public async Task<MedicalRecord> GetByPatientAsync(Guid patientId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(mr => mr.PatientId == patientId);
        }
        public async Task<MedicalRecord> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.MedicalRecords.FirstOrDefaultAsync(mr => mr.DoctorId == doctorId);
        }
        public async Task<MedicalRecord> GetMedicalRecordByIdAsync(Guid id)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .Include(mr => mr.Diagnoses)
                    .ThenInclude(d => d.Treatments)
                .Include(mr => mr.Prescriptions)
                    .ThenInclude(p => p.Items)
                        .ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(mr => mr.Id == id);
        }
        public async Task CreateMedicalRecordAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateMedicalRecordAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Update(medicalRecord);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteByIdAsync(Guid id)
        {
            var medicalRecord = await _context.MedicalRecords.FirstOrDefaultAsync(mr => mr.Id == id);
            if (medicalRecord != null)
            {
                _context.MedicalRecords.Remove(medicalRecord);
                await _context.SaveChangesAsync();
            }
            else 
            { 
                throw new Exception("Medical record not found.");
            }
        }
    }
}
