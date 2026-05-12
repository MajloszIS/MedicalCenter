using System.Security.Claims;
using MedicalCenter.Data;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        private readonly IDeliveryService _deliveryService;
        private readonly AppDbContext _context;

        public CourierController(IDeliveryService deliveryService, AppDbContext context)
        {
            _deliveryService = deliveryService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();
            var userId = Guid.Parse(userIdStr);

            // Szukamy ID kuriera na podstawie zalogowanego konta
            var courier = await _context.Couriers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (courier == null) return NotFound("Nie znaleziono profilu kuriera.");

            // Pobieramy obie listy
            var availableDeliveries = await _deliveryService.GetAvailableDeliveriesAsync();
            var myDeliveries = await _deliveryService.GetMyDeliveriesAsync(courier.Id);

            // Dostępne wysyłamy przez ViewBag, Moje idą w modelu
            ViewBag.AvailableDeliveries = availableDeliveries;

            return View(myDeliveries);
        }

        [HttpPost]
        public async Task<IActionResult> Accept(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();
            var userId = Guid.Parse(userIdStr);

            var courier = await _context.Couriers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (courier != null)
            {
                await _deliveryService.AcceptDeliveryAsync(id, courier.Id);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid deliveryId, string statusName)
        {
            await _deliveryService.ChangeStatusAsync(deliveryId, statusName);
            return RedirectToAction(nameof(Index));
        }
    }
}