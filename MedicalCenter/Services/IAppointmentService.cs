using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IAppointmentService
    {
        public Task<List<AppointmentDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        public Task<List<PatientDto>> GetPatientsByDoctorIdAsync(Guid doctorId);
        public Task<List<AppointmentDto>> GetAppointmentsByPatientIdAsync(Guid patientId);
        public Task CreateAppointmentAsync(Guid doctorId, Guid patientId, DateTime appointmentDate, string description, string? notes);
    }
}
