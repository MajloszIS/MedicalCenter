using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class AppointmentRepository : IAppointmentRepository 
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Status)
                .ToListAsync();
        }
        public Task<Appointment> GetAppointmentByIdAsync(Guid id)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public Task CreateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            return _context.SaveChangesAsync();
        }
        public Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return _context.SaveChangesAsync();
        }
        public async Task DeleteAppointmentAsync(Guid id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
