using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
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
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISpecializationsRepository _specializationsRepository;
        private readonly IPatientRepository _patientRepository; 

        public AdminController(IDoctorRepository doctorRepository, IUserRepository userRepository, ISpecializationsRepository specializationsRepository, IPatientRepository patientRepository)
        {
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _specializationsRepository = specializationsRepository;
            _patientRepository = patientRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Doctors()
        {
            var doctors = await _doctorRepository.GetAllDoctorsAsync();

            var doctorDtos = doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FirstName = d.User.FirstName,
                LastName = d.User.LastName,
                Phone = d.User.Phone,
                SpecializationName = d.Specialization.Name
            }).ToList();

            return View(doctorDtos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateDoctorDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                ViewBag.Error = "Konto z tym Email już istnieje";
                return View();
            }

            if (ModelState.IsValid)
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Phone = dto.Phone,
                    PasswordHash = passwordHash,
                    RoleId = 2
                };

                await _userRepository.CreateUserAsync(user);

                // Pobierz specjalizację na podstawie nazwy
                var specialization = await _specializationsRepository.GetSpecializationByNameAsync(dto.SpecializationName);
                if (specialization == null)
                {
                    ViewBag.Error = "Nie znaleziono takiej specjalizacji";
                    return View();
                }

                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    LicenseNumber = dto.LicenseNumber,
                    SpecializationId = specialization.Id,
                    UserId = user.Id
                };

                doctor.UserId = user.Id;

                await _doctorRepository.CreateDoctorAsync(doctor);

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
            var doctor = await _doctorRepository.GetDoctorByIdAsync(DoctorId);

            if (doctor != null)
            {
                var user = await _userRepository.GetUserByIdAsync(doctor.UserId);

                await _doctorRepository.DeleteDoctorAsync(doctor.Id);

                if (user != null)
                {
                    await _userRepository.DeleteUserAsync(user.Id);
                }
            }

            return RedirectToAction("Doctors");
        }

        public async Task<IActionResult> Patients()
        {
            var patients = await _patientRepository.GetAllPatientsAsync();

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
