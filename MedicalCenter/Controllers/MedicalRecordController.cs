using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IDoctorService _doctorService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService, IDoctorService doctorService)
        {
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Index(Guid patientId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _doctorService.GetDoctorByUserIdAsync(Guid.Parse(userId));

            var medicalRecord = await _medicalRecordService.GetOrCreateAsync(doctor.Id, patientId);

            return View(medicalRecord);
        }
    }
}
