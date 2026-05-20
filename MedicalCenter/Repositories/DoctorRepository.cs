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
        public async Task<Doctor> GetDoctorByIdAsync(Guid id)
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.Id == id);

            return doctors;
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
        public async Task DeleteDoctorAsync(Guid id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.DoctorDepartments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return;

            _context.DoctorDepartments.RemoveRange(doctor.DoctorDepartments);
            _context.Doctors.Remove(doctor);

            await _context.SaveChangesAsync();

        }
        public async Task<Doctor> GetDoctorByUserIdAsync(Guid userId)
        {
            return await _context.Doctors.Include(d => d.User).Include(d => d.Specialization).FirstOrDefaultAsync(d => d.UserId == userId);
        }
        public async Task<List<Doctor>> GetDoctorsBySpecializationAsync(string specializationName)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .Where(d => d.Specialization.Name == specializationName)
                .ToListAsync();
        }
    }
}
