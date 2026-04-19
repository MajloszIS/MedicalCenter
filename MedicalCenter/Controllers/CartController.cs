using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Patient")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IPatientService _patientService;

        public CartController(ICartService cartService, IPatientService patientService)
        {
            _cartService = cartService;
            _patientService = patientService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();
            var userId = Guid.Parse(userIdString);

            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null) return NotFound();

            await _cartService.CreateOrderFromCartAsync(patient.Id);

            TempData["SuccessMessage"] = "Zamówienie zostało złożone pomyślnie!";

            return RedirectToAction("Index", "Medicines");
        }
    }
}