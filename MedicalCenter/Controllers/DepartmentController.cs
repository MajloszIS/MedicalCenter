using MedicalCenter.DTOs;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        readonly IDepartmentService _departmentService;
        public DepartmentController (IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDepartment(DepartmentDto departmentDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Niepoprawne dane.";
                return RedirectToAction("Index");

            }

            try
            {
                await _departmentService.AddDepartmentAsync(departmentDto);
                TempData["Success"] = "Pomyślnie dodano department";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(Guid departmentId)
        {
            var review = await _departmentService.GetDepartmentByIdAsync(departmentId);
            if (review == null) return NotFound();

            try
            {
                await _departmentService.DeleteDepartmentAsync(departmentId);
                TempData["Success"] = "Pomyślnie usunięto Departament";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(DepartmentDto departmentDto)
        {
            var review = await _departmentService.GetDepartmentByIdAsync(departmentDto.Id);
            if (review == null) return NotFound();

            try
            {
                await _departmentService.EditDepartmentAsync(departmentDto);
                TempData["Success"] = "Pomyślnie edytowano Departament";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("Index");
            }
        }

    }
}
