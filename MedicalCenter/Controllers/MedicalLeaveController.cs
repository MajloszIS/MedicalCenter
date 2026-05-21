using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Controllers
{
    public class MedicalLeaveController : Controller
    {
        private readonly IMedicalLeaveService _medicalLeaveService;
        private readonly IPatientService _patientService;

        public MedicalLeaveController(IMedicalLeaveService medicalLeaveService, IPatientService patientService) 
        { 
            _medicalLeaveService = medicalLeaveService;
            _patientService = patientService;
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Create(Guid patientId)
        {
            try
            {
                var patient = _patientService.GetPatientByIdAsync(patientId);
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
            try
            {
                await _medicalLeaveService.CreateMedicalLeaveAsync(medicalLeaveDto);
                TempData["Succes"] = "Pomyślnie dodano zwolnienie";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "MedicalRecord", new { patientId = medicalLeaveDto.PatientId });
            }
        }

    }
}
