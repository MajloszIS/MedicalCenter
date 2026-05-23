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
            try
            {
                var medicines = await _medicineService.GetAvailableMedicinesAsync();
                return View(medicines);
            }
            catch
            {
                TempData["ErrorMessage"] = "Wystąpił problem z załadowaniem listy leków. Spróbuj ponownie później.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid medicineId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    TempData["ErrorMessage"] = "Ilość dodawanych leków musi być większa niż zero.";
                    return RedirectToAction("Index");
                }

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
                    return Unauthorized();

                var patient = await _patientService.GetPatientByUserIdAsync(userId);
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Nie znaleziono profilu pacjenta.";
                    return RedirectToAction("Index");
                }

                var result = await _cartService.AddToCartAsync(patient.Id, medicineId, quantity);

                if (result.success)
                {
                    TempData["Success"] = result.message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.message;
                }

                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorMessage"] = "Wystąpił błąd podczas dodawania leku do koszyka.";
                return RedirectToAction("Index");
            }
        }
    }
}