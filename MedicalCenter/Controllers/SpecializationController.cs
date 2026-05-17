using MedicalCenter.Services;
using MedicalCenter.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SpecializationController : Controller
    {
        private readonly ISpecializationService _specializationService;

        public SpecializationController(ISpecializationService specializationService)
        {
            _specializationService = specializationService;
        }

        public async Task<IActionResult> Specializations()
        {
            var specializationDtos = await _specializationService.GetAllSpecializationsAsync();

            return View(specializationDtos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSpecialization(SpecializationDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _specializationService.CreateSpecializationAsync(dto);
                    return RedirectToAction("Specializations", "Specialization");
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                    return RedirectToAction("Specializations");
                }
            }
            else
            {
                TempData["Error"] = "Niepoprawne dane. Proszę poprawić błędy i spróbować ponownie.";
                return RedirectToAction("Specializations");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSpecialization(Guid SpecializationId)
        {
            await _specializationService.DeleteSpecializationAsync(SpecializationId);

            return RedirectToAction("Specializations");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSpecialization(Guid specializationId, SpecializationDto dto)
        {    
            try
            {
                await _specializationService.UpdateSpecializationAsync(specializationId, dto);
                return RedirectToAction("Specializations");
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Specializations");
            }
        }

    }
}
