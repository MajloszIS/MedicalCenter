using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Include(a => a.Status)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
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
        public async Task DeleteAppointmentsByPatientIdAsync(Guid patientId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .ToListAsync();

            _context.Appointments.RemoveRange(appointments);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.Patient)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Include(a => a.Status)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }
        public async Task<List<AppointmentStatus>> GetAllAppointmentStatusAsync()
        {
            return await _context.AppointmentStatuses.ToListAsync();
        }
    }
}
