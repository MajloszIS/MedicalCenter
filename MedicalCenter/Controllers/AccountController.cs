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
        private readonly IDoctorService _doctorService;
        public AccountController(IUserService userService, IPatientService patientService, IDoctorService doctorService)
        {
            _userService = userService;
            _patientService = patientService;
            _doctorService = doctorService;
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

        public async Task<IActionResult> Profile()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (User.IsInRole("Patient"))
                return RedirectToAction("PatientProfile");
            else if (User.IsInRole("Doctor"))
                return RedirectToAction("DoctorProfile");
            else
                return RedirectToAction("Register");
        }

        public async Task<IActionResult> PatientProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");
                
            var patientProfile = await _patientService.GetPatientProfileAsync(Guid.Parse(userId));

            return View(patientProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientProfile(string newPassword) 
        {
          
            return View();
        }

        public async Task<IActionResult> DoctorProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var doctorProfile = await _doctorService.GetDoctorProfileAsync(Guid.Parse(userId));

            return View(doctorProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                ViewBag.Error = "Nie wybrano pliku";
                return RedirectToAction("Profile");
            }

            // sprawdzenie rozszerzen
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(profilePicture.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ViewBag.Error = "Dozwolone tylko JPG i PNG";
                return RedirectToAction("Profile");
            }

            // unikalna nazwa pliku
            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine("wwwroot", "images", "profiles");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // zapisz plik
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            // zapisz ścieżkę w bazie
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _userService.UpdateProfilePictureAsync(Guid.Parse(userId), $"/images/profiles/{fileName}");

            return RedirectToAction("PatientProfile");
        }
    }
}
