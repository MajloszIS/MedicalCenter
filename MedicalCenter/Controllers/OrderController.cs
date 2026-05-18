using MedicalCenter.Services;
using MedicalCenter.Repositories;
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
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();

            var patient = await _patientRepository.GetPatientByUserIdAsync(Guid.Parse(userIdStr));
            if (patient == null) return NotFound("Nie znaleziono pacjenta.");

            var orders = await _orderService.GetPatientOrdersAsync(patient.Id);
            return View(orders);
        }
    }
}