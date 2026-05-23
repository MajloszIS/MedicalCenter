using MedicalCenter.DTOs;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        private readonly IDepartmentService _departmentService;
        public AdminController(
            IPatientService patientService, 
            IDoctorService doctorService, 
            IUserService userService, 
            IAppointmentService appointmentService, 
            ICourierService courierService,
            IOrderService orderService,
            IMedicineService medicineService,
            IDeliveryService deliveryService,
            IDepartmentService departmentService
            )
        {
            _doctorService = doctorService;
            _userService = userService;
            _patientService = patientService;
            _appointmentService = appointmentService;
            _courierService = courierService;
            _orderService = orderService;
            _medicineService = medicineService;
            _deliveryService = deliveryService;
            _departmentService = departmentService;
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
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = departments;
            var specializations = await _doctorService.GetAllSpecializationsAsync();
            return View(specializations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(AdminCreateDoctorDto dto)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.IsUserWithThisEmailExists(dto.Email))
                {
                    ViewBag.Error = "Konto z tym Email już istnieje";
                    var specs = await _doctorService.GetAllSpecializationsAsync();
                    var departments = await _departmentService.GetAllDepartmentsAsync();
                    ViewBag.Departments = departments;
                    return View(specs);
                }

                try
                {
                    await _doctorService.CreateDoctorAsync(dto);
                    return RedirectToAction("Doctors", "Admin");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex;
                    return RedirectToAction("Doctors");
                }
            }
            else
            {
                ViewBag.Error = "Niepoprawne dane. Proszę poprawić błędy i spróbować ponownie.";

                var departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Departments = departments;
                var specs = await _doctorService.GetAllSpecializationsAsync();
                return View(specs);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDoctor(Guid doctorId)
        {
            try
            {
                await _doctorService.DeleteDoctorAsync(doctorId);
                TempData["Succes"] = "Pomyślnie usunięto lekarza";
                return RedirectToAction("Doctors");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Doctors");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditDoctor(Guid doctorId)
        {
            try
            {
                var doctorUserId = await _userService.GetUserIdByDoctorIdAsync(doctorId);

                var doctorProfile = await _doctorService.GetDoctorProfileAsync(doctorUserId);

                var specializations = await _doctorService.GetAllSpecializationsAsync();
                var allDepartments = await _departmentService.GetAllDepartmentsAsync();

                ViewBag.DoctorId = doctorId;
                ViewBag.Specializations = specializations;
                ViewBag.Departments = allDepartments;
                ViewBag.DoctorDepartmentIds = doctorProfile.SelectedDepartment.Select(x => x.Id).ToList();

                return View(doctorProfile);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Doctors");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(Guid doctorId, UpdateDoctorProfileDto updateDoctorProfileDto)
        {
            try
            {
                var doctorUserId = await _userService.GetUserIdByDoctorIdAsync(doctorId);
                await _doctorService.UpdateDoctorProfileAsync(doctorUserId, updateDoctorProfileDto);
                TempData["Succes"] = "Pomyślnie edytowano lekarza";
                return RedirectToAction("Doctors");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Doctors");
            }

        }

        [HttpGet]
        public async Task<IActionResult> DoctorAppointments(Guid doctorId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);

                ViewBag.DoctorName = $"{doctor.FirstName} {doctor.LastName}";
                return View(appointments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Doctors");
            }
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
            try
            {
                await _patientService.DeletePatientAsync(patientId);
                TempData["Succes"] = "Pomyślnie usunięto pacjenta";
                return RedirectToAction("Patients");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Patients");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditPatient(Guid patientId)
        {
            try
            {
                var patientUserId = await _userService.GetUserIdByPatientIdAsync(patientId);
                var patientProfile = await _patientService.GetPatientProfileAsync(patientUserId);

                ViewBag.PatientId = patientId;
                return View(patientProfile);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Patients");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(Guid patientId, UpdatePatientProfileDto updatePatientProfileDto)
        {
            try
            {
                var patientUserId = await _userService.GetUserIdByPatientIdAsync(patientId);
                await _patientService.UpdatePatientProfileAsync(patientUserId, updatePatientProfileDto);
                TempData["Succes"] = "Pomyślnie edytowano pacjenta";
                return RedirectToAction("Patients");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Patients");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PatientAppointments(Guid patientId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
                var patient = await _patientService.GetPatientByIdAsync(patientId);

                ViewBag.PatientName = $"{patient.FirstName} {patient.LastName}";
                return View(appointments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Patients");
            }
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
        public async Task<IActionResult> CreateCourier(AdminCreateCourierDto dto)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.IsUserWithThisEmailExists(dto.Email))
                {
                    ViewBag.Error = "Konto z tym Email już istnieje";
                    return View();
                }

                try
                {
                    await _courierService.CreateCourierAsync(dto);
                    return RedirectToAction("Couriers", "Admin");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex;
                    return RedirectToAction("Couriers", "Admin");
                }
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
            try
            { 
                await _courierService.DeleteCourierAsync(courierId);
                TempData["Succes"] = "Pomyślnie usunięto kuriera";
                return RedirectToAction("Couriers");
            }
            catch 
            {
                TempData["ErrorMessage"] = "Nie można znaleźć kuriera.";
                return RedirectToAction("Couriers");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCourier(Guid courierId)
        {
            try
            {
                var courierUserId = await _userService.GetUserIdByCourierIdAsync(courierId);
                var courierProfile = await _courierService.GetCourierProfileAsync(courierUserId);

                ViewBag.CourierId = courierId;
                return View(courierProfile);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Couriers");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourier(Guid courierId, UpdateCourierProfileDto updateProfileDto)
        {
            try
            {
                var courierUserId = await _userService.GetUserIdByCourierIdAsync(courierId);
                await _courierService.UpdateCourierProfileAsync(courierUserId, updateProfileDto);
                TempData["Succes"] = "Pomyślnie edytowano kuriera";
                return RedirectToAction("Couriers");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Couriers");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CourierDeliveries(Guid courierId)
        {
            try
            {
                var courier = await _courierService.GetCourierByIdAsync(courierId);
                ViewBag.CourierName = $"{courier.FirstName} {courier.LastName}";
                var deliveries = await _deliveryService.GetMyDeliveriesAsync(courierId);
                return View(deliveries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Couriers");
            }
        }

        // Apteka
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
            if (ModelState.IsValid)
            {
                try
                {
                    await _medicineService.AddMedicineAsync(dto);
                    TempData["Success"] = "Nowy lek został pomyślnie dodany!";
                    return RedirectToAction("Medicines");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("Medicines");
                }
            }

            ViewBag.Error = "Niepoprawne dane. Proszę poprawić błędy i spróbować ponownie.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicine(Guid medicineId)
        {
            try
            {
                await _medicineService.DeleteMedicineAsync(medicineId);
                TempData["Success"] = "Lek został pomyślnie usunięty z bazy.";
                return RedirectToAction("Medicines");
            }
            catch
            {
                TempData["ErrorMessage"] = "Nie można znaleźć leku";
                return RedirectToAction("Medicines");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditMedicine(Guid medicineId)
        {
            try
            {
                var dto = await _medicineService.GetMedicineForEditAsync(medicineId);
                var categories = await _medicineService.GetAllCategoriesAsync();

                ViewBag.Categories = categories;
                ViewBag.MedicineId = medicineId;
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Medicines");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicine(UpdateMedicineDto dto, Guid medicineId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dto.Id = medicineId;
                    await _medicineService.UpdateMedicineAsync(dto);

                    TempData["Success"] = "Dane leku zostały zaktualizowane!";
                    return RedirectToAction("Medicines");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("Medicines");
                }
            }
            else
            {
                var categories = await _medicineService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;
                ViewBag.MedicineId = medicineId;
                TempData["ErrorMessage"] = "Niepoprawne dane. Proszę poprawić błędy i spróbować ponownie.";

                return View(dto);
            }

        }
    }
}
