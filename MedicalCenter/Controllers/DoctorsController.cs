using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors
                .Include(a => a.User)
                .Include(d => d.Specialization)
                .ToListAsync();

            var doctorDto = doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FirstName = d.User.FirstName,
                LastName = d.User.LastName,
                Phone = d.User.Phone,
                SpecializationName = d.Specialization.Name
            }).ToList();

            return View(doctorDto);
        }
    }
}
