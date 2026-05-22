using MedicalCenter.Models;
using MedicalCenter.Repositories;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Patient")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPatientRepository _patientRepository;

        public OrderController(IOrderService orderService, IPatientRepository patientRepository)
        {
            _orderService = orderService;
            _patientRepository = patientRepository;
        }

        public async Task<IActionResult> MyOrders()
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr)) return Challenge();

                var patient = await _patientRepository.GetPatientByUserIdAsync(Guid.Parse(userIdStr));
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Nie znaleziono pacjenta.";
                    return RedirectToAction("Index", "Home");
                }

                var orders = await _orderService.GetPatientOrdersAsync(patient.Id);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Nie udało się pobrać historii zamówień: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Review(Guid orderId)
        {
            ViewBag.OrderId = orderId;
            return View("OrderReview", new OrderRating { OrderId = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(OrderRating model)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr)) return Challenge();

                var patient = await _patientRepository.GetPatientByUserIdAsync(Guid.Parse(userIdStr));
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Nie znaleziono pacjenta.";
                    return RedirectToAction("MyOrders");
                }

                model.PatientId = patient.Id;
                model.CreatedAt = DateTime.UtcNow;

                await _orderService.AddOrderRatingAsync(model);

                TempData["Success"] = "Dziękujemy za ocenę zamówienia!";
                return RedirectToAction("MyOrders");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Nie udało się dodać opinii: " + ex.Message;
                return RedirectToAction("MyOrders");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadInvoice(Guid orderId)
        {
            try
            {
                var invoice = await _orderService.GetInvoiceByOrderIdAsync(orderId);
                if (invoice == null)
                {
                    TempData["InfoMessage"] = "Faktura nie została jeszcze wygenerowana lub zamówienie nie zostało opłacone.";
                    return RedirectToAction("MyOrders");
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (invoice.PatientUserId.ToString() != currentUserId)
                {
                    TempData["ErrorMessage"] = "Brak uprawnień do pobrania tej faktury.";
                    return RedirectToAction("MyOrders");
                }

                byte[] pdfBytes = _orderService.GenerateInvoicePdf(invoice);
                string fileName = $"Faktura_{invoice.OrderNumber}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Wystąpił błąd podczas generowania faktury: " + ex.Message;
                return RedirectToAction("MyOrders");
            }
        }
    }
}