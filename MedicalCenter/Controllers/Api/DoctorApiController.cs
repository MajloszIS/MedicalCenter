using MedicalCenter.Models;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers.Api
{
    [Authorize(AuthenticationSchemes = "Bearer")]
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

        [HttpGet("specialization/{specializationName}")]
        public async Task<IActionResult> GetDoctorsBySpecialization(string specializationName)
        {
            var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specializationName);
            if (doctors == null || !doctors.Any())
            {
                return NotFound();
            }

            return Ok(doctors);
        }
    }
}
