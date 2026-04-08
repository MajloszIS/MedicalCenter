using MedicalCenter.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicalCenter.Models;
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
            return View(await _context.Doctors.Include(a => a.User).Include(d => d.Specialization).ToListAsync());
        }
    }
}
