using MedicalCenter.DTOs;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Controllers.Api
{
    // Dostęp tylko dla ról administracyjnych i kurierskich
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Courier-")]
    [ApiController]
    [Route("api/deliveries")]
    public class DeliveryApiController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryApiController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        // GET: api/deliveries/unassigned
        [HttpGet("unassigned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUnassignedDeliveries()
        {
            var deliveries = await _deliveryService.GetAvailableDeliveriesAsync();

            if (deliveries == null || !deliveries.Any())
            {
                return Ok(new List<object>());
            }

            return Ok(deliveries);
        }

        // POST: api/deliveries/{id}/accept
        [HttpPost("{id}/accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AcceptDelivery(Guid id, [FromBody] AcceptDeliveryDto dto)
        {
            if (dto == null || dto.CourierId == Guid.Empty)
            {
                return BadRequest(new { message = "Nie podano prawidłowego ID kuriera w ciele zapytania." });
            }

            try
            {
                await _deliveryService.AcceptDeliveryAsync(id, dto.CourierId);
                return Ok(new { message = "Dostawa została pomyślnie przypisana." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Nie udało się przypisać dostawy. Być może została już przypisana innej osobie.", details = ex.Message });
            }
        }

        // PATCH: api/deliveries/{id}/status
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDeliveryStatus(Guid id, [FromBody] UpdateDeliveryStatusDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.StatusName))
            {
                return BadRequest(new { message = "Nazwa statusu jest wymagana." });
            }

            try
            {
                await _deliveryService.ChangeStatusAsync(id, dto.StatusName);
                return Ok(new { message = $"Status dostawy został pomyślnie zmieniony na: {dto.StatusName}" });
            }
            catch (Exception)
            {
                return StatusCode(420, "Nie znaleziono dostawy o podanym identyfikatorze.");
            }
        }
    }
}