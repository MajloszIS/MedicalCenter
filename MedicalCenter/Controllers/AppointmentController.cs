using MedicalCenter.DTOs;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Logout", "Account");
            }
            var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("Logout", "Account");
            }
            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patient.Id);

            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid DoctorId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(DoctorId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doctor not found.";
                return RedirectToAction("Index", "Doctors");
            }
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid DoctorId, DateTime appointmentDate, string Description, string? Notes)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(DoctorId);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Lekarz nie został znaleziony.";
                return RedirectToAction("Index", "Doctors");
            }

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Logout", "Account");
                }

                var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Patient not found.";
                    return RedirectToAction("Index", "Home");
                }

                if (appointmentDate == DateTime.MinValue || appointmentDate < DateTime.Now)
                {
                    TempData["ErrorMessage"] = "Wybierz datę wizyty.";
                    return View(doctor);
                }

                await _appointmentService.CreateAppointmentAsync(DoctorId, patient.Id, appointmentDate, Description, Notes);

                return RedirectToAction(nameof(Index));
            }

            return View(doctor);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid AppointmentId)
        {
            var statuses = await _appointmentService.GetAllAppointmentStatusAsync();
            ViewBag.Statuses = statuses;
            var appointment = await _appointmentService.GetAppointmentByIdAsync(AppointmentId);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index", "Home");
            }    
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid AppointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Logout", "Account");
            }

            var appointment = await _appointmentService.GetAppointmentByIdAsync(AppointmentId);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Wizyta nie została znaleziona.";
                return RedirectToAction(nameof(Index));
            }

            var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
            if (patient == null || appointment.Patient.Id != patient.Id)
            {
                TempData["ErrorMessage"] = "Brak uprawnień do anulowania tej wizyty.";
                return RedirectToAction(nameof(Index));
            }

            await _appointmentService.CancelAppointmentAsync(AppointmentId);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(Guid AppointmentId, string note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }
            await _appointmentService.AddNoteAsync(AppointmentId, note);
            return RedirectToAction("Details", new { AppointmentId = AppointmentId });
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentStatus(Guid AppointmentId, Guid statusId)
        {
            await _appointmentService.UpdateAppointmentStatusAsync(AppointmentId, statusId);
            return RedirectToAction("Details", new { AppointmentId = AppointmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RescheduleAppointment(Guid AppointmentId, DateTime? newDate)
        {
            if (newDate == null || newDate == DateTime.MinValue)
            {
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }

            await _appointmentService.RescheduleAppointmentAsync(AppointmentId, newDate.Value);
            return RedirectToAction("Details", new { AppointmentId = AppointmentId });
        }

        [Authorize(Roles = "Patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentDescription(Guid AppointmentId, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }
            await _appointmentService.UpdateAppointmentDescriptionAsync(AppointmentId, description);
            return RedirectToAction("Details", new { AppointmentId = AppointmentId });
        }
    }
}
