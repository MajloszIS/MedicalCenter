using MedicalCenter.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    public class DiagnosisController : Controller
    {
        private readonly IDiagnosisService _diagnosisService;
        private readonly IPatientService _patientService;
        public DiagnosisController(IDiagnosisService diagnosisService, IPatientService patientService) 
        {
            _diagnosisService = diagnosisService;
            _patientService = patientService;
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
                TempData["ErrorMessage"] = "Nie znaleziono Użytkownika";
                return await LogoutAndRedirect();
            }

            try
            {
                var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
                var diagnoses = await _diagnosisService.GetPatientDiagnosisAsync(patient.Id);
                return View(diagnoses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
