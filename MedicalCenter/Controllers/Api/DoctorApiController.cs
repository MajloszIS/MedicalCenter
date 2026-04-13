using MedicalCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorApiController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorApiController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();

            return Ok(doctors);
        }
    }
}
