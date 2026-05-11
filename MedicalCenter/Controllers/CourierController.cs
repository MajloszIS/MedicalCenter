using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        private readonly IDeliveryService _deliveryService;

        public CourierController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deliveries = await _deliveryService.GetAllDeliveriesAsync();

            return View(deliveries);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Guid deliveryId, string statusName)
        {
            await _deliveryService.ChangeStatusAsync(deliveryId, statusName);
            return RedirectToAction(nameof(Index));
        }
    }
}