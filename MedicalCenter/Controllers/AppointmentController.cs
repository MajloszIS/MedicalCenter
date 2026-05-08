using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MedicalCenter.Services;

namespace MedicalCenter.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        public AppointmentController(IPatientService patientService, IAppointmentService appointmentService, IDoctorService doctorService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _doctorService = doctorService;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patient.Id);

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid DoctorId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(DoctorId);
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid DoctorId, DateTime appointmentDate, string Description, string? Notes)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));

                await _appointmentService.CreateAppointmentAsync(DoctorId, patient.Id, appointmentDate, Description, Notes);

                return RedirectToAction(nameof(Index));
            }

            var doctor = await _doctorService.GetDoctorByIdAsync(DoctorId);
            return View(doctor);
            return View();
        }

        public async Task<IActionResult> Details(Guid AppointmentId)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(AppointmentId);
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid AppointmentId)
        {
            await _appointmentService.CancelAppointmentAsync(AppointmentId);
            return RedirectToAction(nameof(Index));
        }
    }
}
