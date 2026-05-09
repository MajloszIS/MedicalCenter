using MedicalCenter.Data;
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

        public Task<List<Patient>> GetAllPatientsAsync()
        {     
            return _context.Patients.Include(p => p.User).ToListAsync(); 
        }
        public Task<Patient> GetPatientByIdAsync(Guid id)
        {     
            return _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id); 
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
        public async Task<Patient> GetPatientByUserIdAsync(Guid userId)
        {
            return await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
