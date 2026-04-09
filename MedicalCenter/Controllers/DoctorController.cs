using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

namespace MedicalCenter.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == Guid.Parse(userId));

            return View(await _context.Appointments
                .Include(a => a.Status)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Where(a => a.DoctorId == doctor.Id)
                .ToListAsync());
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Patients()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == Guid.Parse(userId));

            var patients = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctor.Id)
                .Select(a => a.Patient)
                .Distinct()
                .ToListAsync();

            return View(patients);
        }
    }
}
