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

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index(Guid patientId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));

            var medicalRecord = await _medicalRecordService.GetOrCreateAsync(doctor.Id, patientId);

            var medicines = await _medicineService.GetAvailableMedicinesAsync();
            ViewBag.Medicines = medicines;

            return View(medicalRecord);
        }


        // Metoda do dodawania Diagnozy
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

        // Metoda do usuwania Diagnozy
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDiagnosis(Guid diagnosisId, Guid patientId)
        {
            await _diagnosisService.DeleteDiagnosisAsync(diagnosisId);
            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do dodawnia leczenia do diagnozy
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddTreatment(Guid medicalRecordId, Guid patientId)
        {
            ViewBag.MedicalRecordId = medicalRecordId;
            ViewBag.PatientId = patientId;
            return View();
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTreatment(Guid diagnosisId, Guid patientId, string description)
        {
            var treatmentDto = new TreatmentDto
            {
                Description = description,
                DiagnosisId = diagnosisId   
            };
            await _treatmentService.CreateTreatmentAsync(treatmentDto);

            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do usuwania leczenia z diagnozy
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTreatment(Guid treatmentId, Guid patientId)
        {
            await _treatmentService.DeleteTreatmentAsync(treatmentId);
            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do dodawnia recepty
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddPrescription(Guid medicalRecordId, Guid patientId)
        {
            ViewBag.MedicalRecordId = medicalRecordId;
            ViewBag.PatientId = patientId; 

            return View();
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrescription(Guid medicalRecordId, Guid patientId, List<Guid> medicineIds, List<int> quantities)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));

            foreach (var id in medicineIds)
            {
                Console.WriteLine($"MedicineId: {id}");
            }

            var prescriptionDto = new PrescriptionDto
            {
                MedicalRecordId = medicalRecordId,
                DoctorId = doctor.Id,
                Items = medicineIds.Select((id, index) => new PrescriptionItemDto { MedicineId = id, Quantity = quantities[index] }).ToList()
            };
            await _prescriptionService.CreatePrescription(prescriptionDto);

            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do usuwania recepty
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePrescription(Guid prescriptionId, Guid patientId)
        {
            await _prescriptionService.DeletePrescription(prescriptionId);
            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }

        // Metoda do dodawnia leków z recept
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePrescriptionItem(Guid itemId, Guid patientId)
        {
            await _prescriptionService.DeletePrescriptionItemAsync(itemId);
            return RedirectToAction("Index", "MedicalRecord", new { patientId = patientId });
        }
    }
}
