using Microsoft.AspNetCore.Mvc;
using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Controllers
{
    public class PatientsController : Controller
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Patients.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient, string Email, string FirstName, string LastName, string Phone, string PasswordHash)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = Phone,
                    PasswordHash = PasswordHash, // na razie plain text, później hashowanie
                    RoleId = 3 // zakładam że 3 = Patient, dostosuj do swojej tabeli ról
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                patient.Id = Guid.NewGuid();
                patient.UserId = user.Id;

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(patient);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            return View(patient);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}