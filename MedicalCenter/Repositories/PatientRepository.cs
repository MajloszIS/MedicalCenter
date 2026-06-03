using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;
        public PatientRepository(AppDbContext context) 
        {
            _context = context;
        }

        public Task<List<Patient>> GetAllPatientsAsync(int skip = 0, int take = int.MaxValue)
        {     
            return _context.Patients
                .Include(p => p.User)
                .OrderBy(p => p.User.FirstName)
                .Skip(skip)
                .Take(take)
                .ToListAsync(); 
        }
        public async Task<int> GetPatientsCountAsync()
        {
            return await _context.Patients.CountAsync();
        }
        public async Task<Patient?> GetPatientByIdAsync(Guid id)
        {     
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.Id == id); 

            return patient;
        }
        public Task CreatePatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            return _context.SaveChangesAsync();
        }
        public Task UpdatePatientAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            return _context.SaveChangesAsync();
        }
        public async Task DeletePatientAsync(Guid id)
        {
            var patient = _context.Patients.FirstOrDefault(d => d.Id == id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Patient?> GetPatientByUserIdAsync(Guid userId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return patient;
        }
        public async Task<List<PatientDemographicsDto>> GetPatientDemographicsAsync(int ageFrom, int ageTo)
        {
            return await _context.PatientDemographics
                .FromSqlRaw("SELECT * FROM rpt.fn_PatientDemographics({0}, {1})", ageFrom, ageTo)
                .ToListAsync();
        }
    }
}
