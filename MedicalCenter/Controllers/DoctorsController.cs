using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalCenter.Services;

namespace MedicalCenter.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();

            return View(doctors);
        }
    }
}
