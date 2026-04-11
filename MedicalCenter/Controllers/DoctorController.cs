using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        public DoctorController(IDoctorRepository doctorRepository, IAppointmentRepository appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(Guid.Parse(userId));

            var appointments = await _appointmentRepository.GetAppointmentsByDoctorIdAsync(doctor.Id);

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

            return View(appointmentDto);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Patients()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(Guid.Parse(userId));

            var patients = await _appointmentRepository.GetPatientsByDoctorIdAsync(doctor.Id);

            var patientDto = patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Phone = p.User.Phone,
                Pesel = p.Pesel
            }).ToList();

            return View(patientDto);
        }
    }
}
