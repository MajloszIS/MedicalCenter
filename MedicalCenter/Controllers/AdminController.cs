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

        public async Task<IActionResult> Doctors()
        {
            var doctorDtos = await _doctorService.GetAllDoctorsAsync();

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
            if (await _userService.IsUserWithThisEmailExists(dto.Email))
            {
                ViewBag.Error = "Konto z tym Email już istnieje";
                return View();
            }

            if (ModelState.IsValid)
            {
                await _doctorService.CreateDoctorAsync(dto);

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
            await _doctorService.DeleteDoctorAsync(DoctorId);

            return RedirectToAction("Doctors");
        }

        public async Task<IActionResult> Patients()
        {
            var patients = await _patientService.GetAllPatientsAsync();

            return View(patients);
        }
    }
}
