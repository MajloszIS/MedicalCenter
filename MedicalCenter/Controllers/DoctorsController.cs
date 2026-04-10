using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorsController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorRepository.GetAllDoctorsAsync();

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
