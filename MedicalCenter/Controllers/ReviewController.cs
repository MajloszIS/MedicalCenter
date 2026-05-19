using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Patient")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        public ReviewController(IReviewService reviewService, IDoctorService doctorService, IPatientService patientService) 
        {       
            _reviewService = reviewService;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        [HttpGet]
        public  async Task<IActionResult> AddReview(Guid doctorId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(ReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Niepoprawne dane.";
                var doctor = await _doctorService.GetDoctorByIdAsync(reviewDto.DoctorId);
                return View(doctor);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login", "Account");

            var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
            if (patient == null) return RedirectToAction("Index", "Home");

            reviewDto.PatientId = patient.Id;

            try
            {
                await _reviewService.AddReviewAsync(reviewDto);
                TempData["Success"] = "Pomyślnie dodano opinię";
                return RedirectToAction("Index", "Doctors");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                var doctor = await _doctorService.GetDoctorByIdAsync(reviewDto.DoctorId);
                return View(doctor);
            }        
        }
    }
}
