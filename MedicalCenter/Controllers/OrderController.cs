using MedicalCenter.Data;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Patient")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPatientRepository _patientRepository;

        public OrderController(
            IOrderService orderService,
            IPatientRepository patientRepository
            )
        {
            _orderService = orderService;
            _patientRepository = patientRepository;
        }

        public async Task<IActionResult> MyOrders()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();

            var patient = await _patientRepository.GetPatientByUserIdAsync(Guid.Parse(userIdStr));
            if (patient == null) return NotFound("Nie znaleziono pacjenta.");

            var orders = await _orderService.GetPatientOrdersAsync(patient.Id);
            return View(orders);
        }
        [HttpGet]
        public IActionResult Review(Guid orderId)
        {
            ViewBag.OrderId = orderId;
            return View("OrderReview",new OrderRating { OrderId = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(OrderRating model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();

            var patient = await _patientRepository.GetPatientByUserIdAsync(Guid.Parse(userIdStr));
            if (patient == null) return NotFound("Nie znaleziono pacjenta.");

            model.PatientId = patient.Id;
            model.CreatedAt = DateTime.UtcNow;

            await _orderService.AddOrderRatingAsync(model);

            TempData["Success"] = "Dziękujemy za ocenę zamówienia!";
            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> DownloadInvoice(Guid orderId)
        {
            // Pobranie danych
            var invoice = await _orderService.GetInvoiceByOrderIdAsync(orderId);
            if (invoice == null) return NotFound("Faktura nie została jeszcze wygenerowana.");

            // Walidacja uprawnień (czy pacjent pobiera SWOJĄ fakturę)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (invoice.PatientUserId.ToString() != currentUserId) return Unauthorized();

            // Zlecenie wygenerowania pliku PDF serwisowi
            byte[] pdfBytes = _orderService.GenerateInvoicePdf(invoice);
            string fileName = $"Faktura_{invoice.OrderNumber}.pdf";

            // Zwrócenie pliku użytkownikowi
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}