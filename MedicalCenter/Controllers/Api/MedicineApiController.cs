using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicalCenter.Services;

namespace MedicalCenter.Controllers.Api
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineApiController : Controller
    {
        private readonly IMedicineService _medicineService;
        public MedicineApiController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMedicine()
        {
            var medicines = await _medicineService.GetAllMedicineAsync();
            return Ok(medicines);
        }
    }
}
