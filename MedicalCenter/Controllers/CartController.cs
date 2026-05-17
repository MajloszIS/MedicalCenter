using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;

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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();
            var userId = Guid.Parse(userIdString);

            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null) return NotFound();

            var cart = await _cartService.GetCartAsync(patient.Id);

            return View(cart);
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

            var cart = await _cartService.GetCartAsync(patient.Id);
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index");

            var domain = "http://localhost:5029";

            var lineItems = new List<SessionLineItemOptions>();
            foreach (var item in cart.Items)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Medicine.Price * 100),
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Medicine.Name,
                        },
                    },
                    Quantity = item.Quantity,
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card", "blik", "p24" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = domain + "/Cart/OrderSuccess",
                CancelUrl = domain + "/Cart/OrderCancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);


            await _cartService.CreateOrderFromCartAsync(patient.Id);
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }
        [HttpGet]
        public IActionResult OrderSuccess(string sessionId)
        {
            TempData["SuccessMessage"] = "Płatność zaakceptowana! Twoje zamówienie jest w drodze.";
            return View();
        }

        [HttpGet]
        public IActionResult OrderCancel()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(Guid medicineId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();
            var userId = Guid.Parse(userIdString);

            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null) return NotFound();

            await _cartService.RemoveFromCartAsync(patient.Id, medicineId);

            TempData["SuccessMessage"] = "Przedmiot został usunięty z koszyka.";
            return RedirectToAction("Index");
        }
    }
}