using Humanizer;
using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalCenter.Controllers
{
    public class AccountController : Controller
    {

        private readonly IUserService _userService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IAddressService _addressService;
        private readonly ICourierService _courierService;
        private readonly IDepartmentService _departmentService;
        public AccountController(IUserService userService, IPatientService patientService, IDoctorService doctorService, IAddressService addressService, ICourierService courierService, IDepartmentService departmentService)
        {
            _userService = userService;
            _patientService = patientService;
            _doctorService = doctorService;
            _addressService = addressService;
            _courierService = courierService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _userService.LoginAsync(dto);

                var identity = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new (ClaimTypes.Email, result.Email),
                    new Claim(ClaimTypes.Name, $"{result.FullName}"),
                    new (ClaimTypes.Role, result.RoleName)
                }, "login");

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ViewBag.Error = "Nieprawidłowy email lub hasło";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PatientRegisterDto dto)
        {
            if (await _userService.IsUserWithThisEmailExists(dto.Email))
            {
                ViewBag.Error = "Konto z tym Email już istnieje";
                return View();
            }

            if (ModelState.IsValid)
            {
                await _patientService.RegisterAsync(dto);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (User.IsInRole("Patient"))
                return RedirectToAction("PatientProfile");
            else if (User.IsInRole("Doctor"))
                return RedirectToAction("DoctorProfile");
            else if (User.IsInRole("Courier"))
                return RedirectToAction("CourierProfile");
            else if (User.IsInRole("Admin"))
                return RedirectToAction("AdminProfile");
            else
            {
                await Logout();
                return RedirectToAction("Register", "Account");
            }
        }


        // Akcje dla profilu pacjenta
        public async Task<IActionResult> PatientProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Logout");

            try
            {
                var patientProfile = await _patientService.GetPatientProfileAsync(Guid.Parse(userId));
                return View(patientProfile);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Logout");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePatientProfile(UpdatePatientProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userId == null || userRole == null) return RedirectToAction("Logout");

            try
            {
                await _patientService.UpdatePatientProfileAsync(Guid.Parse(userId), dto);

                var updatedUser = await _patientService.GetPatientProfileAsync(Guid.Parse(userId));
                if (updatedUser == null)
                    return RedirectToAction("Logout");

                var identity = new ClaimsIdentity(new Claim[]
                    {
                    new (ClaimTypes.NameIdentifier, userId),
                    new (ClaimTypes.Email, updatedUser.Email),
                    new Claim(ClaimTypes.Name, $"{updatedUser.FirstName} {updatedUser.LastName}"),
                    new (ClaimTypes.Role, userRole)
                    }, "login");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("PatientProfile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Logout");
            }

        }

        public async Task<IActionResult> PatientAddress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) 
                return RedirectToAction("Logout");

            try
            {
                var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
                var patientAddress = await _addressService.GetAddressByPatientIdAsync(patient.Id);
                return View(patientAddress);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Logout");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePatientAddress(AddressDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) 
                return RedirectToAction("Logout");

            try
            {
                var patient = await _patientService.GetPatientByUserIdAsync(Guid.Parse(userId));
                await _addressService.UpdateAddressAsync(patient.Id, dto);
                return RedirectToAction("PatientProfile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Logout");
            }
        }

        // Akcje dla profilu lekarza
        public async Task<IActionResult> DoctorProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var doctorProfile = await _doctorService.GetDoctorProfileAsync(Guid.Parse(userId));
            var specializations = await _doctorService.GetAllSpecializationsAsync();

            ViewBag.DoctorDepartment = doctorProfile.SelectedDepartment;
            ViewBag.Specializations = specializations;

            return View(doctorProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorProfile(UpdateDoctorProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");
            await _doctorService.UpdateDoctorProfileAsync(Guid.Parse(userId), dto);

            var updatedUser = await _doctorService.GetDoctorProfileAsync(Guid.Parse(userId));

            // Aktulaizacja claimsów po zmianie danych
            var identity = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier, userId),
                    new (ClaimTypes.Email, updatedUser.Email),
                    new Claim(ClaimTypes.Name, $"{updatedUser.FirstName} {updatedUser.LastName}"),
                    new (ClaimTypes.Role, User.FindFirstValue(ClaimTypes.Role))
                }, "login");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("DoctorProfile");
        }

        public async Task<IActionResult> CourierProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var courierProfile = await _courierService.GetCourierProfileAsync(Guid.Parse(userId));
            return View(courierProfile);
        }


        // Akcje dla profilu kuriera
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCourierProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            await _courierService.UpdateCourierProfileAsync(Guid.Parse(userId), dto);
            var updatedUser = await _courierService.GetCourierProfileAsync(Guid.Parse(userId));

            var identity = new ClaimsIdentity(new Claim[]
                {
            new (ClaimTypes.NameIdentifier, userId),
            new (ClaimTypes.Email, updatedUser.Email),
            new Claim(ClaimTypes.Name, $"{updatedUser.FirstName} {updatedUser.LastName}"),
            new (ClaimTypes.Role, User.FindFirstValue(ClaimTypes.Role))
                }, "login");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("CourierProfile");
        }

        // Akcje dla profilu Admina
        public async Task<IActionResult> AdminProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var adminProfile = await _userService.GetUserProfileAsync(Guid.Parse(userId));
            return View(adminProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdminProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");
            await _userService.UpdateProfileAsync(Guid.Parse(userId), dto);
            var updatedUser = await _userService.GetUserProfileAsync(Guid.Parse(userId));

            // Aktulaizacja claimsów po zmianie danych
            var identity = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier, userId),
                    new (ClaimTypes.Email, updatedUser.Email),
                    new Claim(ClaimTypes.Name, $"{updatedUser.FirstName} {updatedUser.LastName}"),
                    new (ClaimTypes.Role, User.FindFirstValue(ClaimTypes.Role))
                }, "login");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("AdminProfile");
        }


        // Akcje dostępne dla wszystkich ról
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _userService.ChangePasswordAsync(Guid.Parse(userId), oldPassword, newPassword);
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                ViewBag.Error = "Nie wybrano pliku";
                return RedirectToAction("Profile");
            }

            // sprawdzenie rozszerzen
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(profilePicture.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ViewBag.Error = "Dozwolone tylko JPG i PNG";
                return RedirectToAction("Profile");
            }

            // unikalna nazwa pliku
            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine("wwwroot", "images", "profiles");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // zapisz plik
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            // zapisz ścieżkę w bazie
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _userService.UpdateProfilePictureAsync(Guid.Parse(userId), $"/images/profiles/{fileName}");

            return RedirectToAction("Profile");
        }

        // Logowanie się przez Google
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/Account/GoogleCallback"
            };
            return Challenge(properties, "Google");
        }

        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded) return RedirectToAction("Login");

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var firstName = result.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = result.Principal.FindFirstValue(ClaimTypes.Surname);

            // sprawdź czy user już istnieje
            var user = await _userService.IsUserWithThisEmailExists(email);

            if (!user)
            {
                // utwórz nowego usera bez hasła
                await _patientService.RegisterGoogleUserAsync(email, firstName, lastName);
            }

            // zaloguj przez cookie
            var userWithRole = await _userService.GetUserByEmailWithRoleAsync(email);
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, userWithRole.Id.ToString()),
                new (ClaimTypes.Email, userWithRole.Email),
                new (ClaimTypes.Name, $"{userWithRole.FirstName} {userWithRole.LastName}"),
                new (ClaimTypes.Role, userWithRole.RoleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
    }
}
