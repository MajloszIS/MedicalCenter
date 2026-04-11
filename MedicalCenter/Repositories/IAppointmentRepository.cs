using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IAppointmentRepository
    {
        public Task<List<Appointment>> GetAllAppointmentsAsync();
        public Task<Appointment> GetAppointmentByIdAsync(Guid id);
        public Task CreateAppointmentAsync(Appointment appointment);
        public Task UpdateAppointmentAsync(Appointment appointment);
        public Task DeleteAppointmentAsync(Guid id);
    }
}
