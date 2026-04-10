using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Doctors()
        {
            return View(await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Email, string FirstName, string LastName, string Phone, string Password, string LicenseNumber, string SpecializationName)
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
                    RoleId = 2
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var specialization = await _context.Specializations.FirstOrDefaultAsync(s => s.Name == SpecializationName);
                if (specialization == null)
                {
                    ViewBag.Error = "Nie znaleziono takiej specjalizacji";
                    return View();
                }

                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    LicenseNumber = LicenseNumber,
                    SpecializationId = specialization.Id,
                    UserId = user.Id
                };

                doctor.UserId = user.Id;

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                return RedirectToAction("Doctors", "Admin");
            }

            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid DoctorId)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == DoctorId);

            if (doctor != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == doctor.UserId);

                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();

                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Doctors");
        }

        public async Task<IActionResult> Patients()
        {
            return View(await _context.Patients
                .Include(d => d.User)
                .ToListAsync());
        }
    }
}
