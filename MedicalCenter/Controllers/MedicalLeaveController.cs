using MedicalCenter.DTOs;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    public class MedicalLeaveController : Controller
    {
        private readonly IMedicalLeaveService _medicalLeaveService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public MedicalLeaveController(IMedicalLeaveService medicalLeaveService, IPatientService patientService, IDoctorService doctorService) 
        { 
            _medicalLeaveService = medicalLeaveService;
            _patientService = patientService;
            _doctorService = doctorService;
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
                return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId} );
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(MedicalLeaveDto medicalLeaveDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Logout", "Account");

            var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));
            if (doctor == null)
                return RedirectToAction("Logout", "Account");
            medicalLeaveDto.DoctorId = doctor.Id;

            try
            {
                await _medicalLeaveService.CreateMedicalLeaveAsync(medicalLeaveDto);
                TempData["Succes"] = "Pomyślnie dodano zwolnienie";
                return RedirectToAction("Index", "MedicalRecord", new { patientId = medicalLeaveDto.PatientId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "MedicalRecord", new { patientId = medicalLeaveDto.PatientId });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> MedicalLeaves()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Logout", "Account");

            var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
            if (patient == null)
                return RedirectToAction("Logout", "Account");

            try
            {
                var medicalLeaves = await _medicalLeaveService.GetMedicalLeavesByPatientIdAsync(patient.Id);
                return View(medicalLeaves);
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
                var pdfBytes = await _medicalLeaveService.GenerateMedicalLeavePdfAsync(id);
                return File(pdfBytes, "application/pdf", $"zwolnienie-{id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
