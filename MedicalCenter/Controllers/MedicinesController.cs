using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Patient")]
    public class MedicinesController : Controller
    {
        private readonly IMedicineService _medicineService;
        private readonly ICartService _cartService;
        private readonly IPatientService _patientService;

        public MedicinesController(IMedicineService medicineService, ICartService cartService, IPatientService patientService)
        {
            _medicineService = medicineService;
            _cartService = cartService;
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var medicines = await _medicineService.GetAvailableMedicinesAsync();
            return View(medicines);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid medicineId, int quantity)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();
            var userId = Guid.Parse(userIdString);

            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null) return NotFound();

            var result = await _cartService.AddToCartAsync(patient.Id, medicineId, quantity);

            if (result.success)
            {
                TempData["SuccessMessage"] = result.message;
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
            }

            return RedirectToAction("Index");
        }
    }
}