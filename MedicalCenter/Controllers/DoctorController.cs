using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            var appointments = await _context.Appointments
                .Include(a => a.Status)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialization)
                .Where(a => a.DoctorId == doctor.Id)
                .ToListAsync();

            var appointmentDto = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                Patient = new PatientDto
                {
                    Id = a.Patient.Id,
                    FirstName = a.Patient.User.FirstName,
                    LastName = a.Patient.User.LastName,
                    Phone = a.Patient.User.Phone,
                    Pesel = a.Patient.Pesel
                },
                AppointmentDate = a.AppointmentDate,
                StatusName = a.Status.Name
            }).ToList();

            return View(appointmentDto);
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

            var patientDto = patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Phone = p.User.Phone,
                Pesel = p.Pesel
            }).ToList();

            return View(patientDto);
        }
    }
}
