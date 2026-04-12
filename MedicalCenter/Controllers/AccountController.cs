using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MedicalCenter.Services;

namespace MedicalCenter.Controllers
{
    public class AccountController : Controller
    {

        private readonly IUserService _userService;
        private readonly IPatientService _patientService;
        public AccountController(IUserService userService, IPatientService patientService)
        {
            _userService = userService;
            _patientService = patientService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);

            if (result == null)
            {
                ViewBag.Error = "Nieprawidłowy email lub hasło";
                return View();
            }
            else
            {
                var identity = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new (ClaimTypes.Email, result.Email),
                    new Claim(ClaimTypes.Name, $"{result.FullName}"),
                    new (ClaimTypes.Role, result.RoleName)
                }, "login");

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PatientRegisterDto dto)
        {
            if (await _userService.IsUserWithThisEmailExists(dto.Email))
            {
                ViewBag.Error = "Konto z tym Email już istnieje";
                return View();
            }

            if (ModelState.IsValid)
            {
                await _patientService.RegisterAsync(dto);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
