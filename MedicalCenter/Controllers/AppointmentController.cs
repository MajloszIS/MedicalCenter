using MedicalCenter.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        private async Task<IActionResult> LogoutAndRedirect()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Wylogowano";
                return await LogoutAndRedirect();
            }

            try
            {
                var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
                var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patient.Id);
                return View(appointments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid DoctorId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(DoctorId);
                return View(doctor);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid DoctorId, DateTime appointmentDate, string Description, string? Notes, int DurationTime)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var doctor = await _doctorService.GetDoctorByIdAsync(DoctorId);

                    if (DurationTime <= 0 || DurationTime > 120 || DurationTime % 15 != 0)
                    {
                        ViewBag.Error = "Nieprawidłowa długość wizyty.";
                        return View(doctor);
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null) return await LogoutAndRedirect();
                    var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));


                    if (appointmentDate == DateTime.MinValue || appointmentDate < DateTime.Now)
                    {
                        TempData["ErrorMessage"] = "Wybierz datę wizyty.";
                        return View(doctor);
                    }

                    await _appointmentService.CreateAppointmentAsync(DoctorId, patient.Id, appointmentDate, Description, Notes, DurationTime);
                    TempData["Succes"] = "Utworzono wizytę";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("Create", new { doctorId = DoctorId });
                }
            }

            TempData["ErrorMessage"] = "Wprowadzono nie prawidłowe dane";
            return RedirectToAction("Create", new { doctorId = DoctorId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid AppointmentId)
        {
            try
            {
                var statuses = await _appointmentService.GetAllAppointmentStatusAsync();
                ViewBag.Statuses = statuses;

                var appointment = await _appointmentService.GetAppointmentByIdAsync(AppointmentId);
                return View(appointment);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid AppointmentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return await LogoutAndRedirect();

                var appointment = await _appointmentService.GetAppointmentByIdAsync(AppointmentId);

                var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
                if (patient == null || appointment.Patient.Id != patient.Id)
                {
                    TempData["ErrorMessage"] = "Brak uprawnień do anulowania tej wizyty.";
                    return RedirectToAction(nameof(Index));
                }

                await _appointmentService.CancelAppointmentAsync(AppointmentId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
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

            try
            {
                await _appointmentService.AddNoteAsync(AppointmentId, note);
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentStatus(Guid AppointmentId, int statusId)
        {
            try
            {
                await _appointmentService.UpdateAppointmentStatusAsync(AppointmentId, statusId);
                TempData["Succes"] = "Pomyślnie zmieniono status wizyty";
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RescheduleAppointment(Guid AppointmentId, DateTime? newDate, int DurationTime)
        {
            if (newDate == null || newDate == DateTime.MinValue)
            {
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }

            try
            {
                await _appointmentService.RescheduleAppointmentAsync(AppointmentId, newDate.Value, DurationTime);
                TempData["Succes"] = "Pomyślnie zmieniono Datę wizyty";
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }
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
            
            try
            {
                await _appointmentService.UpdateAppointmentDescriptionAsync(AppointmentId, description);
                return RedirectToAction("Details", new { AppointmentId = AppointmentId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
