using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BCrypt.Net;

namespace MedicalCenter.Controllers
{
    public class AccountController : Controller
    {

        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                ViewBag.Error = "Nieprawidłowy email";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                ViewBag.Error = "Nieprawidłowe hasło";
                return View();
            }
            else
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role.Name)
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
        public async Task<IActionResult> Register(string Email, string FirstName, string LastName, string Phone, string Password, DateTime BirthDate, string Pesel)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email); 
            if (existingUser != null)
            {
                ViewBag.Error = "Konto z tym Email już istnieje";
                return View();
            }

            if (ModelState.IsValid)
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = Phone,
                    PasswordHash = passwordHash,
                    RoleId = 3
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    BirthDate = BirthDate,
                    Pesel = Pesel
                };

                patient.UserId = user.Id;

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
