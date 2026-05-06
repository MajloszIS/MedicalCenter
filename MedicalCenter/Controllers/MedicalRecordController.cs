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

        public MedicalRecordController(IMedicalRecordService medicalRecordService, IDoctorService doctorService, IDiagnosisService diagnosisService, IMedicineService medicineService, IPrescriptionService prescriptionService)
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
            _diagnosisService = diagnosisService;
            _medicineService = medicineService;
            _prescriptionService = prescriptionService;
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
    }
}
