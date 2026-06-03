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

        // GET: api/doctors/{doctorId}/workload?dateFrom=2024-01-01&dateTo=2024-12-31
        [HttpGet("{doctorId}/workload")]
        public async Task<IActionResult> GetDoctorWorkload(Guid doctorId, [FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            var result = await _doctorService.GetDoctorWorkloadAsync(doctorId, dateFrom, dateTo);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}