using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface IAppointmentService
    {
        public Task<List<AppointmentDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        public Task<List<PatientDto>> GetPatientsByDoctorIdAsync(Guid doctorId);
        public Task<List<AppointmentDto>> GetAppointmentsByPatientIdAsync(Guid patientId);
        public Task CreateAppointmentAsync(Guid doctorId, Guid patientId, DateTime appointmentDate, string description, string? notes);
        public Task<AppointmentDto> GetAppointmentByIdAsync(Guid appointmentId);
        public Task CancelAppointmentAsync(Guid appointmentId);
        public Task AddNoteAsync(Guid appointmentId, string note);
        public Task<List<AppointmentStatusDto>> GetAllAppointmentStatusAsync();
        public Task UpdateAppointmentStatusAsync(Guid appointmentId, Guid statusId);
        public Task RescheduleAppointmentAsync(Guid appointmentId, DateTime newDate);
        public Task UpdateAppointmentDescriptionAsync(Guid appointmentId, string description);


    }
}
