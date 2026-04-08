using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));

            return View(await _context.Appointments
                .Include(a => a.Status)
                .Include(a => a.Doctor)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Where(a => a.PatientId == patient.Id)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid DoctorId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));
                var appointment = new Appointment {
                    PatientId = patient.Id, 
                    DoctorId = DoctorId,
                    StatusId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                    AppointmentDate = DateTime.Now
                };
               
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }
    }
}
