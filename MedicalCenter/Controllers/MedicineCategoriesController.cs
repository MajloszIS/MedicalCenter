using MedicalCenter.DTOs;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MedicineCategoriesController : Controller
    {
        private readonly IMedicineService _medicineService;

        public MedicineCategoriesController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _medicineService.GetAllCategoriesAsync();
                return View(categories);
            }
            catch
            {
                TempData["Error"] = "Wystąpił problem z pobraniem listy kategorii. Spróbuj ponownie później.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicineCategory(MedicineCreateCategoryDTO dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _medicineService.AddCategoryAsync(dto);
                    TempData["SuccessMessage"] = $"Kategoria '{dto.Name}' została dodana!";
                }
                else
                {
                    TempData["Error"] = "Nazwa kategorii jest niepoprawna.";
                }

                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "Wystąpił błąd podczas dodawania kategorii.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicineCategory(Guid id, string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    TempData["Error"] = "Nazwa kategorii nie może być pusta.";
                    return RedirectToAction("Index");
                }

                await _medicineService.UpdateCategoryAsync(id, name);

                TempData["SuccessMessage"] = $"Nazwa kategorii została zmieniona na '{name}'.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicineCategory(Guid id)
        {
            try
            {
                await _medicineService.DeleteCategoryAsync(id);

                TempData["SuccessMessage"] = "Kategoria została pomyślnie usunięta.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}