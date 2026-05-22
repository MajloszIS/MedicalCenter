using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
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

        [Authorize(Roles = "Patient")]
        [HttpGet]
        public async Task<IActionResult> AddReview(Guid doctorId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                return View(doctor);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Doctors");
            }
        }

        [Authorize(Roles = "Patient")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DoctorReviews(Guid doctorId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (doctor == null) return NotFound();

            try
            {
                var reviews = await _reviewService.GetReviewsByDoctorIdAsync(doctorId);
                ViewBag.DoctorName = doctor.FirstName + " " + doctor.LastName;
                return View(reviews);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("Doctors", "Admin");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null) return NotFound();

            try
            {
                await _reviewService.DeleteReviewAsync(reviewId);
                TempData["Success"] = "Pomyślnie usunięto opinię";
                
                var reviews = await _reviewService.GetReviewsByDoctorIdAsync(review.DoctorId);
                if (reviews == null || !reviews.Any())
                {
                    TempData["InfoMessage"] = "Brak opinii dla tego lekarza.";
                    return RedirectToAction("Doctors", "Admin");
                }
                else
                {
                    return RedirectToAction("DoctorReviews", new { doctorId = review.DoctorId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("Doctors", "Admin");
            }
        }
    }
}
