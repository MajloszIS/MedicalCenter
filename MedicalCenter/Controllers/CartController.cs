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
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
                    return Unauthorized();

                var patient = await _patientService.GetPatientByUserIdAsync(userId);
                if (patient == null) return NotFound();

                var cart = await _cartService.GetCartAsync(patient.Id);
                return View(cart);
            }
            catch
            {
                TempData["ErrorMessage"] = "Wystąpił problem z załadowaniem koszyka. Spróbuj ponownie później.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
                    return Unauthorized();

                var patient = await _patientService.GetPatientByUserIdAsync(userId);
                if (patient == null) return NotFound();

                var stockCheck = await _cartService.ValidateCartStockAsync(patient.Id);
                if (!stockCheck.valid)
                {
                    TempData["ErrorMessage"] = stockCheck.message;
                    return RedirectToAction("Index");
                }

                var cart = await _cartService.GetCartAsync(patient.Id);
                if (cart == null || !cart.Items.Any()) return RedirectToAction("Index");

                var domain = $"{Request.Scheme}://{Request.Host}";
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
                    SuccessUrl = domain + "/Cart/OrderSuccess?sessionId={CHECKOUT_SESSION_ID}",
                    CancelUrl = domain + "/Cart/OrderCancel",
                };

                var service = new SessionService();
                Session session = service.Create(options);

                await _cartService.CreateOrderFromCartAsync(patient.Id, session.Id);

                return Redirect(session.Url);
            }
            catch (StripeException)
            {
                TempData["ErrorMessage"] = "Wystąpił problem z bramką płatniczą. Spróbuj ponownie później.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorMessage"] = "Nie udało się rozpocząć procesu płatności. Skontaktuj się z obsługą.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderSuccess(string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    TempData["ErrorMessage"] = "Brak identyfikatora sesji płatności.";
                    return RedirectToAction("Index", "Home");
                }

                await _cartService.ConfirmPaymentAsync(sessionId);
                return View();
            }
            catch
            {
                TempData["ErrorMessage"] = "Wystąpił problem z potwierdzeniem Twojego zamówienia. Jeśli środki zostały pobrane, skontaktuj się z nami.";
                return RedirectToAction("Index", "Home");
            }
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
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
                    return Unauthorized();

                var patient = await _patientService.GetPatientByUserIdAsync(userId);
                if (patient == null) return NotFound();

                await _cartService.RemoveFromCartAsync(patient.Id, medicineId);

                TempData["Success"] = "Przedmiot został usunięty z koszyka.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["ErrorMessage"] = "Nie udało się usunąć przedmiotu z koszyka.";
                return RedirectToAction("Index");
            }
        }
    }
}