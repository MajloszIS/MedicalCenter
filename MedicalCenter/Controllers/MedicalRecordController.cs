using MedicalCenter.Services;
using MedicalCenter.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IDoctorService _doctorService;
        private readonly IDiagnosisService _diagnosisService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService, IDoctorService doctorService, IDiagnosisService diagnosisService)
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
            _diagnosisService = diagnosisService;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index(Guid patientId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));

            var medicalRecord = await _medicalRecordService.GetOrCreateAsync(doctor.Id, patientId);

            return View(medicalRecord);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddDiagnosis(Guid medicalRecordId, Guid patientId)
        {
            ViewBag.MedicalRecordId = medicalRecordId;
            ViewBag.PatientId = patientId;
            return View();
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiagnosis(Guid medicalRecordId, Guid patientId, string description)
        {
            var diagnosisDto = new DiagnosisDto
            {
                Description = description,
                MedicalRecordId = medicalRecordId
            };
            await _diagnosisService.CreateDiagnosisAsync(diagnosisDto);

            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }
    }
}
