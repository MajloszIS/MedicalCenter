using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    public class ReferralController : Controller
    {
        private readonly IMedicalLeaveService _medicalLeaveService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IReferralService _referralService;

        public ReferralController(IMedicalLeaveService medicalLeaveService, IPatientService patientService, IDoctorService doctorService, IReferralService referralService)
        {
            _referralService = referralService;
            _medicalLeaveService = medicalLeaveService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        private async Task<IActionResult> LogoutAndRedirect()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Create(Guid patientId)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdAsync(patientId);
                return View(patient);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(ReferralDto referralDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return await LogoutAndRedirect();

            var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));
            if (doctor == null)
                return await LogoutAndRedirect();
            referralDto.DoctorId = doctor.Id;

            try
            {
                await _referralService.CreateReferralAsync(referralDto);
                TempData["Succes"] = "Pomyślnie dodano skierowanie";
                return RedirectToAction("Index", "MedicalRecord", new { patientId = referralDto.PatientId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "MedicalRecord", new { patientId = referralDto.PatientId });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> Referrals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return await LogoutAndRedirect();

            var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
            if (patient == null)
                return await LogoutAndRedirect();

            try
            {
                var referrals = await _referralService.GetReferralsByPatientIdAsync(patient.Id);
                return View(referrals);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DownloadPdf(Guid id)
        {
            try
            {
                var pdfBytes = await _referralService.GenerateReferralPdfAsync(id);
                return File(pdfBytes, "application/pdf", $"skierowanie-{id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
