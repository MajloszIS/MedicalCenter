using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Controllers.Api
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/doctors")]
    public class DoctorApiController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorApiController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: api/doctors
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            if (doctors == null || !doctors.Any())
            {
                return Ok(new List<object>());
            }
            return Ok(doctors);
        }

        // GET: api/doctors/specializations/{specializationName}
        [HttpGet("specializations/{specializationName}")]
        public async Task<IActionResult> GetDoctorsBySpecialization(string specializationName)
        {
            var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specializationName);
            if (doctors == null || !doctors.Any())
            {
                return Ok(new List<object>());
            }
            return Ok(doctors);
        }
    }
}