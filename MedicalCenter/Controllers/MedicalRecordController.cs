using MedicalCenter.DTOs;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IDoctorService _doctorService;
        private readonly IDiagnosisService _diagnosisService;
        private readonly IMedicineService _medicineService;
        private readonly IPrescriptionService _prescriptionService;
        private readonly ITreatmentService _treatmentService;

        public MedicalRecordController
            (
            IMedicalRecordService medicalRecordService, 
            IDoctorService doctorService, 
            IDiagnosisService diagnosisService, 
            IMedicineService medicineService, 
            IPrescriptionService prescriptionService,
            ITreatmentService treatmentService
            )
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
            _diagnosisService = diagnosisService;
            _medicineService = medicineService;
            _prescriptionService = prescriptionService;
            _treatmentService = treatmentService;
        }

        private async Task<IActionResult> LogoutAndRedirect()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Index(Guid patientId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Wylogowano";
                return await LogoutAndRedirect();
            }

            try
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));
                var medicalRecord = await _medicalRecordService.GetOrCreateAsync(doctor.Id, patientId);
                var medicines = await _medicineService.GetAvailableMedicinesAsync();
                ViewBag.Medicines = medicines;

                return View(medicalRecord);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Patients", "Doctor");
            }

        }

        // Metoda do dodawania Diagnozy
        public async Task<IActionResult> AddDiagnosis(Guid medicalRecordId, Guid patientId)
        {
            ViewBag.MedicalRecordId = medicalRecordId;
            ViewBag.PatientId = patientId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiagnosis(Guid patientId, DiagnosisDto diagnosisDto)
        {
            await _diagnosisService.CreateDiagnosisAsync(diagnosisDto);
            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do usuwania Diagnozy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDiagnosis(Guid diagnosisId, Guid patientId)
        {
            await _diagnosisService.DeleteDiagnosisAsync(diagnosisId);
            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do dodawnia leczenia do diagnozy
        public async Task<IActionResult> AddTreatment(Guid medicalRecordId, Guid patientId)
        {
            ViewBag.MedicalRecordId = medicalRecordId;
            ViewBag.PatientId = patientId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTreatment(Guid patientId, TreatmentDto treatmentDto)
        {
            await _treatmentService.CreateTreatmentAsync(treatmentDto);
            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do usuwania leczenia z diagnozy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTreatment(Guid treatmentId, Guid patientId)
        {
            await _treatmentService.DeleteTreatmentAsync(treatmentId);
            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do dodawnia recepty
        public async Task<IActionResult> AddPrescription(Guid medicalRecordId, Guid patientId)
        {
            ViewBag.MedicalRecordId = medicalRecordId;
            ViewBag.PatientId = patientId; 

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrescription(Guid medicalRecordId, Guid patientId, List<Guid> medicineIds, List<int> quantities, List<string> notes)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Wylogowano";
                return await LogoutAndRedirect();
            }
            if (medicineIds == null || !medicineIds.Any())
            {
                TempData["ErrorMessage"] = "Recepta musi zawierać przynajmniej jeden lek.";
                return RedirectToAction("Index", new { patientId });
            }

            try
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));
                var prescriptionDto = new PrescriptionDto
                {
                    MedicalRecordId = medicalRecordId,
                    DoctorId = doctor.Id,
                    Items = medicineIds.Select((id, index) => new PrescriptionItemDto { MedicineId = id, Quantity = quantities[index], Notes = notes[index] }).ToList()
                };
                await _prescriptionService.CreatePrescription(prescriptionDto);

                return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { patientId = patientId });
            }
        }

        // Metoda do usuwania recepty
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePrescription(Guid prescriptionId, Guid patientId)
        {
            try
            {
                await _prescriptionService.DeletePrescription(prescriptionId);
                return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { patientId = patientId });
            }
        }

        // Metoda do dodawnia leków z recept
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePrescriptionItem(Guid itemId, Guid patientId)
        {
            try
            {
                await _prescriptionService.DeletePrescriptionItemAsync(itemId);
                return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { patientId = patientId });
            }
        }
    }
}
