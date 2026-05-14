using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Numerics;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public AdminController(IPatientService patientService, IDoctorService doctorService, IUserService userService)
        {
            _doctorService = doctorService;
            _userService = userService;
            _patientService = patientService;
        }
        public IActionResult Index()
        {
            return View();
        }

        // Lekarze
        public async Task<IActionResult> Doctors()
        {
            var doctorDtos = await _doctorService.GetAllDoctorsAsync();

            return View(doctorDtos);
        }

        public async Task<IActionResult> CreateDoctor()
        {
            var specializations = await _doctorService.GetAllSpecializationsAsync();
            return View(specializations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(AdminCreateDoctorDto dto)
        {
            if (await _userService.IsUserWithThisEmailExists(dto.Email))
            {
                ViewBag.Error = "Konto z tym Email już istnieje";
                return View();
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine($"Email: {dto.Email}, FirstName: {dto.FirstName}, Spec: {dto.SpecializationName}");

                await _doctorService.CreateDoctorAsync(dto);

                return RedirectToAction("Doctors", "Admin");
            }
            else
            {
                ViewBag.Error = "Niepoprawne dane. Proszę poprawić błędy i spróbować ponownie.";
                var specs = await _doctorService.GetAllSpecializationsAsync();
                return View(specs);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDoctor(Guid doctorId)
        {
            await _doctorService.DeleteDoctorAsync(doctorId);

            return RedirectToAction("Doctors");
        }

        [HttpGet]
        public async Task<IActionResult> EditDoctor(Guid doctorId)
        {
            var doctorUserId = await _userService.GetUserIdByDoctorIdAsync(doctorId);
            if (doctorUserId == Guid.Empty)
            {
                TempData["Error"] = "Nie można znaleźć użytkownika powiązanego z lekarzem.";
                return RedirectToAction("Doctors", "Admin");
            }

            var doctorProfile = await _doctorService.GetDoctorProfileAsync(doctorUserId);
            if (doctorProfile == null)
            {
                TempData["Error"] = "Nie można znaleźć lekarza.";
                return RedirectToAction("Doctors", "Admin");
            }

            var specializations = await _doctorService.GetAllSpecializationsAsync();
            ViewBag.Specializations = specializations;
            ViewBag.DoctorId = doctorId;
            return View(doctorProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(Guid doctorId, UpdateDoctorProfileDto updateDoctorProfileDto)
        {
            var doctorUserId = await _userService.GetUserIdByDoctorIdAsync(doctorId);
            if(doctorUserId == Guid.Empty)
            {
                TempData["Error"] = "Nie można znaleźć użytkownika powiązanego z lekarzem.";
                return RedirectToAction("Doctors", "Admin");
            }

            await _doctorService.UpdateDoctorProfileAsync(doctorUserId ,updateDoctorProfileDto);

            return RedirectToAction("Doctors");
        }

        // Pacjenci
        public async Task<IActionResult> Patients()
        {
            var patients = await _patientService.GetAllPatientsAsync();

            return View(patients);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatient(Guid patientId)
        {
            await _patientService.DeletePatientAsync(patientId);

            return RedirectToAction("Patients");
        }

        [HttpGet]
        public async Task<IActionResult> EditPatient(Guid patientId)
        {
            var patientUserId = await _userService.GetUserIdByPatientIdAsync(patientId);
            if (patientUserId == Guid.Empty)
            {
                TempData["Error"] = "Nie można znaleźć użytkownika powiązanego z pacjentem.";
                return RedirectToAction("Patients", "Admin");
            }

            var patientProfile = await _patientService.GetPatientProfileAsync(patientUserId);
            if (patientProfile == null)
            {
                TempData["Error"] = "Nie można znaleźć pacjenta.";
                return RedirectToAction("Patients", "Admin");
            }

            ViewBag.PatientId = patientId;
            return View(patientProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(Guid patientId, UpdatePatientProfileDto updatePatientProfileDto)
        {
            var patientUserId = await _userService.GetUserIdByPatientIdAsync(patientId);
            if (patientUserId == Guid.Empty)
            {
                TempData["Error"] = "Nie można znaleźć użytkownika powiązanego z pacjentem.";
                return RedirectToAction("Patients", "Admin");
            }

            await _patientService.UpdatePatientProfileAsync(patientUserId, updatePatientProfileDto);

            return RedirectToAction("Patients");
        }
    }
}
