using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;


namespace MedicalCenter.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public async Task<List<AppointmentDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorIdAsync(doctorId);

            var appointmentDto = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                Patient = new PatientDto
                {
                    Id = a.Patient.Id,
                    FirstName = a.Patient.User.FirstName,
                    LastName = a.Patient.User.LastName,
                    Phone = a.Patient.User.Phone,
                    Pesel = a.Patient.Pesel
                },
                AppointmentDate = a.AppointmentDate,
                StatusName = a.Status.Name
            }).ToList();
            
            return appointmentDto;
        }
        public async Task<List<PatientDto>> GetPatientsByDoctorIdAsync(Guid doctorId)
        {
            var patients = await _appointmentRepository.GetPatientsByDoctorIdAsync(doctorId);

            var patientDto = patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Phone = p.User.Phone,
                Pesel = p.Pesel
            }).ToList();

            return patientDto;
        }
        public async Task<List<AppointmentDto>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientIdAsync(patientId);

            var appointmentDto = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                Doctor = new DoctorDto
                {
                    Id = a.Doctor.Id,
                    FirstName = a.Doctor.User.FirstName,
                    LastName = a.Doctor.User.LastName,
                    Phone = a.Doctor.User.Phone,
                    SpecializationName = a.Doctor.Specialization.Name
                },
                AppointmentDate = a.AppointmentDate,
                StatusName = a.Status.Name
            }).ToList();

            return appointmentDto;
        }
        public async Task CreateAppointmentAsync(Guid doctorId, Guid patientId, DateTime appointmentDate, string? description, string? notes)
        {
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                Description = description ?? string.Empty,
                Notes = notes ?? string.Empty,
                StatusId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                AppointmentDate = appointmentDate
            };

            await _appointmentRepository.CreateAppointmentAsync(appointment);
        }

        public async Task<AppointmentDto> GetAppointmentByIdAsync(Guid appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }
            var appointmentDto = new AppointmentDto
            {
                Id = appointment.Id,
                Doctor = new DoctorDto
                {
                    Id = appointment.Doctor.Id,
                    FirstName = appointment.Doctor.User.FirstName,
                    LastName = appointment.Doctor.User.LastName,
                    Phone = appointment.Doctor.User.Phone,
                    SpecializationName = appointment.Doctor.Specialization.Name
                },
                Patient = new PatientDto
                {
                    Id = appointment.Patient.Id,
                    FirstName = appointment.Patient.User.FirstName,
                    LastName = appointment.Patient.User.LastName,
                    Phone = appointment.Patient.User.Phone,
                    Pesel = appointment.Patient.Pesel
                },
                AppointmentDate = appointment.AppointmentDate,
                StatusName = appointment.Status.Name,
                Description = appointment.Description,
                Notes = appointment.Notes ?? "Brak notatek"
            };
            return appointmentDto;
        }
        public async Task CancelAppointmentAsync(Guid appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }
            appointment.StatusId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
        }
        public async Task AddNoteAsync(Guid appointmentId, string note)
        { 
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }
            appointment.Notes = note;
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
        }
        public async Task<List<AppointmentStatusDto>> GetAllAppointmentStatusAsync()
        {
            var statuses = await _appointmentRepository.GetAllAppointmentStatusAsync();
            return statuses.Select(s => new AppointmentStatusDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }
        public async Task UpdateAppointmentStatusAsync(Guid appointmentId, Guid statusId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }
            
            appointment.StatusId = statusId;
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
        }
        public async Task RescheduleAppointmentAsync(Guid appointmentId, DateTime newDate)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }
            appointment.AppointmentDate = newDate;
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
        }
        public async Task UpdateAppointmentDescriptionAsync(Guid appointmentId, string description)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }
            appointment.Description = description;
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
        }

    }
}
