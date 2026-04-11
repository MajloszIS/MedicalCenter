using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IAppointmentRepository
    {
        public Task<List<Appointment>> GetAllAppointmentsAsync();
        public Task<Appointment> GetAppointmentByIdAsync(Guid id);
        public Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        public Task CreateAppointmentAsync(Appointment appointment);
        public Task UpdateAppointmentAsync(Appointment appointment);
        public Task DeleteAppointmentAsync(Guid id);
        public Task<List<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId);
        public Task<List<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);
    }
}
