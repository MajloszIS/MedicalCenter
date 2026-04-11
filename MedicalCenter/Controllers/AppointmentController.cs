using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        public AppointmentController(IPatientRepository patientRepository, IAppointmentRepository appointmentRepository)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _patientRepository.GetPatientByUserIdAsync(Guid.Parse(userId));

            var appointments = await _appointmentRepository.GetAppointmentsByPatientIdAsync(patient.Id);
                

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

            return View(appointmentDto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid DoctorId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var patient = await _patientRepository.GetPatientByUserIdAsync(Guid.Parse(userId));
                var appointment = new Appointment {
                    PatientId = patient.Id, 
                    DoctorId = DoctorId,
                    StatusId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                    AppointmentDate = DateTime.Now
                };
               
                await _appointmentRepository.CreateAppointmentAsync(appointment);

                return RedirectToAction(nameof(Index));
            }

            return View();
        }
    }
}
