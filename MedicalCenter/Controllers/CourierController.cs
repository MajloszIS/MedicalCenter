using System.Security.Claims;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        private readonly IDeliveryService _deliveryService;
        private readonly ICourierService _courierService;

        public CourierController(IDeliveryService deliveryService, ICourierService courierService)
        {
            _deliveryService = deliveryService;
            _courierService = courierService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();

            try
            {
                var userId = Guid.Parse(userIdStr);
                var courier = await _courierService.GetCourierByUserIdAsync(userId);
                if (courier == null) return NotFound("Nie znaleziono profilu kuriera.");

                var availableDeliveries = await _deliveryService.GetAvailableDeliveriesAsync();
                var myDeliveries = await _deliveryService.GetMyDeliveriesAsync(courier.Id);

                ViewBag.AvailableDeliveries = availableDeliveries;

                return View(myDeliveries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Accept(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();

            try
            {
                var userId = Guid.Parse(userIdStr);
                var courier = await _courierService.GetCourierByUserIdAsync(userId);

                if (courier != null)
                {
                    await _deliveryService.AcceptDeliveryAsync(id, courier.Id);
                    TempData["Success"] = "Pomyślnie przypisano dostawę.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid deliveryId, string statusName)
        {
            try
            {
                await _deliveryService.ChangeStatusAsync(deliveryId, statusName);
                TempData["Success"] = "Status dostawy został zmieniony.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}