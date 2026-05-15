using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IMedicineService _medicineService;
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientService _patientService;

        public PrescriptionController(IMedicineService medicineService, IPrescriptionService prescriptionService, IPatientService patientService)
        {
            _medicineService = medicineService;
            _prescriptionService = prescriptionService;
            _patientService = patientService;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
            var prescriptions = await _prescriptionService.GetPrescriptionsByPatientIdAsync(patient.Id);

            return View(prescriptions);
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> DownloadPdf(Guid id)
        {
            try
            {
                var pdfBytes = await _prescriptionService.GeneratePrescriptionPdfAsync(id);
                return File(pdfBytes, "application/pdf", $"recepta-{id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
