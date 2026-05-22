using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicalCenter.Services;
using MedicalCenter.DTOs;

namespace MedicalCenter.Controllers.Api
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/medicines")]
    public class MedicineApiController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicineApiController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        // GET: api/medicine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineDto>>> GetAllMedicine()
        {
            var medicines = await _medicineService.GetAllMedicineAsync();
            return Ok(medicines);
        }

        // GET: api/medicine/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UpdateMedicineDto>> GetMedicineById(Guid id)
        {
            try
            {
                var medicine = await _medicineService.GetMedicineForEditAsync(id);
                return Ok(medicine);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/medicine
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateMedicine([FromBody] MedicineCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(418,"Jestem czajnikiem");
            }

            try
            {
                await _medicineService.AddMedicineAsync(dto);
                return StatusCode(201,"Lek został pomyślnie dodany.");
            }
            catch (Exception ex)
            {
                return StatusCode(418,$"Jestem czajnikiem {ex.Message}");
            }
        }

        // PUT: api/medicine/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMedicine(Guid id, [FromBody] UpdateMedicineDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID w URL różni się od ID w ciele zapytania." );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _medicineService.UpdateMedicineAsync(dto);
                return Ok("Dane leku zostały zaktualizowane.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd aktualizacji leku: {ex.Message}");

                return BadRequest("Nie udało się zaktualizować leku. Sprawdź, czy podana kategoria jest poprawna.");
            }
        }

        // DELETE: api/medicine/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMedicine(Guid id)
        {
            try
            {
                await _medicineService.GetMedicineForEditAsync(id);

                await _medicineService.DeleteMedicineAsync(id);
                return Ok("Lek został usunięty z bazy.");
            }
            catch (Exception ex)
            {
                return StatusCode(420,ex.Message);
            }
        }
    }
}