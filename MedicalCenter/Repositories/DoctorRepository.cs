using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;
        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .ToListAsync();
        }
        public Task<Doctor> GetDoctorByIdAsync(Guid id)
        {
            return _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        }
        public Task CreateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            return _context.SaveChangesAsync();
        }
        public Task UpdateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            return _context.SaveChangesAsync();
        }
        public Task DeleteDoctorAsync(Guid id)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.Id == id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                return _context.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }
    }
}
