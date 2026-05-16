using MedicalCenter.DTOs;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthApiController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            if (result == null)
            {
                return Unauthorized(new { message = "Nieprawidłowy email lub hasło" });
            }

            var response = _userService.GenerateJwtToken(result);
            return Ok(response);
        }
    }
}