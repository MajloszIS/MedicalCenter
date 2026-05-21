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
            var categories = await _medicineService.GetAllCategoriesAsync();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicineCategory(MedicineCreateCategoryDTO dto)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicineCategory(Guid id, string name)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicineCategory(Guid id)
        {
            bool isDeleted = await _medicineService.DeleteCategoryAsync(id);

            if (isDeleted)
            {
                TempData["SuccessMessage"] = "Kategoria została pomyślnie usunięta.";
            }
            else
            {
                TempData["Error"] = "Nie można usunąć tej kategorii, ponieważ są do niej przypisane leki. Najpierw zmień kategorię przypisanym lekom, lub usuń leki.";
            }

            return RedirectToAction("Index");
        }
    }
}