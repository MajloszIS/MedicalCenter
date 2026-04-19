using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using MedicalCenter.Services;
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
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        public DoctorController(IDoctorService doctorService, IAppointmentService appointmentService)
        {
            _doctorService = doctorService;
            _appointmentService = appointmentService;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));
            var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctor.Id);

            return View(appointments);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Patients()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));

            var patients = await _appointmentService.GetPatientsByDoctorIdAsync(doctor.Id);

            return View(patients);
        }
    }
}
