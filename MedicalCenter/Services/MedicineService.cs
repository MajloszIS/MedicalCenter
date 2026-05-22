using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepo;

        public MedicineService(IMedicineRepository medicineRepo)
        {
            _medicineRepo = medicineRepo;
        }

        public async Task<List<MedicineDto>> GetAllMedicineAsync()
        {
            var medicines = await _medicineRepo.GetAllMedicinesAsync();
            return medicines.Select(m => new MedicineDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryName = m.Category?.Name ?? "Brak kategorii",
                StockQuantity = m.StockQuantity
            }).ToList();
        }

        public async Task<List<MedicineDto>> GetAvailableMedicinesAsync()
        {
            var medicines = await _medicineRepo.GetAllMedicinesAsync();
            return medicines.Select(m => new MedicineDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryName = m.Category?.Name ?? "Brak kategorii",
                StockQuantity = m.StockQuantity
            }).ToList();
        }

        public async Task AddMedicineAsync(MedicineCreateDto dto)
        {
            var category = await _medicineRepo.GetCategoryByIdAsync(dto.CategoryId)
                           ?? throw new Exception("Nie znaleziono podanej kategorii leku.");

            var medicine = new Medicine
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Description = dto.Description,
                CategoryId = dto.CategoryId
            };

            await _medicineRepo.AddMedicineAsync(medicine);
            await _medicineRepo.SaveChangesAsync();
        }

        public async Task DeleteMedicineAsync(Guid id)
        {
            var medicine = await _medicineRepo.GetByIdAsync(id)
                           ?? throw new Exception("Nie znaleziono leku do usunięcia.");

            await _medicineRepo.DeleteMedicineAsync(medicine);
        }

        public async Task<List<MedicineCategory>> GetAllCategoriesAsync()
        {
            return await _medicineRepo.GetAllCategoriesAsync();
        }

        public async Task<UpdateMedicineDto> GetMedicineForEditAsync(Guid id)
        {
            var medicine = await _medicineRepo.GetByIdAsync(id) ?? throw new Exception("Nie znaleziono leku o podanym identyfikatorze.");

            return new UpdateMedicineDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                Price = medicine.Price,
                StockQuantity = medicine.StockQuantity,
                Description = medicine.Description,
                CategoryId = medicine.CategoryId,
                CategoryName = medicine.Category?.Name
            };
        }
        public async Task UpdateMedicineAsync(UpdateMedicineDto dto)
        {
            var medicine = await _medicineRepo.GetByIdAsync(dto.Id)
                           ?? throw new Exception("Nie znaleziono leku o podanym identyfikatorze.");
            var category = await _medicineRepo.GetCategoryByIdAsync(dto.CategoryId)
                           ?? throw new Exception("Nie znaleziono podanej kategorii leku.");
            medicine.Name = dto.Name;
            medicine.Price = dto.Price;
            medicine.StockQuantity = dto.StockQuantity;
            medicine.Description = dto.Description;
            medicine.CategoryId = dto.CategoryId;

            await _medicineRepo.SaveChangesAsync();
        }

        public async Task AddCategoryAsync(MedicineCreateCategoryDTO dto)
        {
            var category = new MedicineCategory
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

            await _medicineRepo.AddCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(Guid id, string name)
        {
            var category = await _medicineRepo.GetCategoryByIdAsync(id)
                           ?? throw new Exception("Nie znaleziono kategorii do aktualizacji.");

            category.Name = name;
            await _medicineRepo.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            bool hasMedicines = await _medicineRepo.HasMedicinesInCategoryAsync(id);
            if (hasMedicines)
            {
                throw new Exception("Nie można usunąć tej kategorii, ponieważ są do niej przypisane leki. Najpierw zmień kategorię przypisanym lekom, lub usuń leki.");
            }

            var category = await _medicineRepo.GetCategoryByIdAsync(id)
                           ?? throw new Exception("Nie znaleziono kategorii do usunięcia.");

            await _medicineRepo.DeleteCategoryAsync(category);
        }
    }
}