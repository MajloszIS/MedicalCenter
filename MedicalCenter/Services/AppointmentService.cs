using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using System.Numerics;


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
        public async Task CreateAppointmentAsync(Guid doctorId, Guid patientId, DateTime appointmentDate)
        {
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                StatusId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                AppointmentDate = appointmentDate
            };

            await _appointmentRepository.CreateAppointmentAsync(appointment);
        }
    }
}
