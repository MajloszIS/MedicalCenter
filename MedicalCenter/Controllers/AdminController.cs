using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IAppointmentService _appointmentService;
        private readonly ICourierService _courierService;
        private readonly IOrderService _orderService;
        private readonly IMedicineService _medicineService;
        private readonly IDeliveryService _deliveryService;

        public AdminController(
            IPatientService patientService, 
            IDoctorService doctorService, 
            IUserService userService, 
            IAppointmentService appointmentService, 
            ICourierService courierService,
            IOrderService orderService,
            IMedicineService medicineService,
            IDeliveryService deliveryService)
        {
            _doctorService = doctorService;
            _userService = userService;
            _patientService = patientService;
            _appointmentService = appointmentService;
            _courierService = courierService;
            _orderService = orderService;
            _medicineService = medicineService;
            _deliveryService = deliveryService;
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

        [HttpGet]
        public async Task<IActionResult> DoctorAppointments(Guid doctorId)
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
            if (appointments == null)
            {
                TempData["Error"] = "Nie można znaleźć lekarza lub jego wizyt.";
                return RedirectToAction("Doctors", "Admin");
            }

            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
            {
                TempData["Error"] = "Nie można znaleźć lekarza.";
                return RedirectToAction("Doctors", "Admin");
            }

            ViewBag.DoctorName = $"{doctor.FirstName} {doctor.LastName}";
            return View(appointments);
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

        [HttpGet]
        public async Task<IActionResult> PatientAppointments(Guid patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            if (appointments == null)
            {
                TempData["Error"] = "Nie można znaleźć pacjenta lub jego wizyt.";
                return RedirectToAction("Patients", "Admin");
            }

            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                TempData["Error"] = "Nie można znaleźć pacjenta.";
                return RedirectToAction("Patients", "Admin");
            }

            ViewBag.PatientName = $"{patient.FirstName} {patient.LastName}";
            return View(appointments);
        }

        // Couriers

        public async Task<IActionResult> Couriers()
        {
            var courierDtos = await _courierService.GetAllCourierAsync();

            return View(courierDtos);
        }

        
        public async Task<IActionResult> CreateCourier()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourier(AdminCreateDto dto)
        {
            if (await _userService.IsUserWithThisEmailExists(dto.Email))
            {
                ViewBag.Error = "Konto z tym Email już istnieje";
                return View();
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine($"Email: {dto.Email}, FirstName: {dto.FirstName}");

                await _courierService.CreateCourierAsync(dto);

                return RedirectToAction("Couriers", "Admin");
            }
            else
            {
                ViewBag.Error = "Niepoprawne dane. Proszę poprawić błędy i spróbować ponownie.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourier(Guid courierId)
        {
            if (ModelState.IsValid)
            {
                try
                { 
                    await _courierService.DeleteCourierAsync(courierId);
                }
                catch 
                {
                    TempData["Error"] = "Nie można znaleźć kuriera.";
                    return RedirectToAction("Couriers", "Admin");
                }

            }
            return RedirectToAction("Couriers", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditCourier(Guid courierId)
        {
            var courierUserId = await _userService.GetUserIdByCourierIdAsync(courierId);
            if (courierUserId == Guid.Empty)
            {
                TempData["Error"] = "Nie można znaleźć użytkownika powiązanego z kurierem.";
                return RedirectToAction("Couriers", "Admin");
            }

            var courierProfile = await _courierService.GetCourierProfileAsync(courierUserId);
            if (courierProfile == null)
            {
                TempData["Error"] = "Nie można znaleźć kuriera.";
                return RedirectToAction("Couriers", "Admin");
            }

            ViewBag.CourierId = courierId;
            return View(courierProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourier(Guid courierId, UpdateProfileDto updateProfileDto)
        {
            var courierUserId = await _userService.GetUserIdByCourierIdAsync(courierId);
            if (courierUserId == Guid.Empty)
            {
                TempData["Error"] = "Nie można znaleźć użytkownika powiązanego z kurierem.";
                return RedirectToAction("Couriers", "Admin");
            }

            await _courierService.UpdateCourierProfileAsync(courierUserId, updateProfileDto);

            return RedirectToAction("Couriers");
        }

        [HttpGet]
        public async Task<IActionResult> CourierDeliveries(Guid courierId)
        {
            var courier = await _courierService.GetCourierByIdAsync(courierId);
            if (courier != null)
            {
                ViewBag.CourierName = $"{courier.FirstName} {courier.LastName}";
            }

            var deliveries = await _deliveryService.GetMyDeliveriesAsync(courierId);

            return View(deliveries);
        }
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }
        [HttpGet]
        public async Task<IActionResult> Medicines()
        {
            var medicines = await _medicineService.GetAllMedicineAsync();
            return View(medicines);
        }
        [HttpGet]
        public async Task<IActionResult> CreateMedicine()
        {
            var categories = await _medicineService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedicine(MedicineCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _medicineService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                ViewBag.Error = "Niepoprawne dane. Proszę poprawić błędy i spróbować ponownie.";
                return View(dto);
            }

            await _medicineService.AddMedicineAsync(dto);
            TempData["SuccessMessage"] = "Nowy lek został pomyślnie dodany!";

            return RedirectToAction("Medicines", "Admin");
        }
        [HttpGet]
        public async Task<IActionResult> EditMedicine(Guid medicineId)
        {
            var dto = await _medicineService.GetMedicineForEditAsync(medicineId);
            if (dto == null) return NotFound("Nie znaleziono leku.");

            var categories = await _medicineService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            ViewBag.MedicineId = medicineId;

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicine(UpdateMedicineDto dto, Guid medicineId)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _medicineService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;
                ViewBag.MedicineId = medicineId;
                TempData["Error"] = "Niepoprawne dane. Proszę poprawić błędy i spróbować ponownie.";

                return View(dto);
            }

            dto.Id = medicineId;
            await _medicineService.UpdateMedicineAsync(dto);

            TempData["SuccessMessage"] = "Dane leku zostały zaktualizowane!";
            return RedirectToAction("Medicines");
        }
    }
}
