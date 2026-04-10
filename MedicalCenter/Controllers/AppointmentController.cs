using MedicalCenter.Data;
using MedicalCenter.DTOs;
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

            var appointments = await _context.Appointments
                .Include(a => a.Status)
                .Include(a => a.Doctor)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Where(a => a.PatientId == patient.Id)
                .ToListAsync();

            var appointmentDto = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                Doctor = new DoctorDto
                {
                    Id = a.Doctor.Id,
                    FirstName = a.Doctor.User.FirstName,
                    LastName = a.Doctor.User.LastName,
                    Phone = a.Doctor.User.Phone,
                    SpecializationName = a.Doctor.Specialization.Name
                },
                AppointmentDate = a.AppointmentDate,
                StatusName = a.Status.Name
            }).ToList();

            return View(appointmentDto);
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
