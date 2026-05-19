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
            var medicineDtos = medicines.Select(m => new MedicineDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryName = m.Category?.Name ?? "Brak kategorii",
                StockQuantity = m.StockQuantity
            }).ToList();
            return medicineDtos;
        }
        public async Task<List<MedicineDto>> GetAvailableMedicinesAsync()
        {
            var medicines = await _medicineRepo.GetAllMedicinesAsync();

            var medicineDtos = medicines.Select(m => new MedicineDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryName = m.Category?.Name ?? "Brak kategorii",
                StockQuantity = m.StockQuantity
            }).ToList();

            return medicineDtos;
        }
        public async Task AddMedicineAsync(MedicineCreateDto dto)
        {
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

        public async Task<List<MedicineCategory>> GetAllCategoriesAsync()
        {
            return await _medicineRepo.GetAllCategoriesAsync();
        }
        public async Task<UpdateMedicineDto> GetMedicineForEditAsync(Guid id)
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);
            if (medicine == null) return null;

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
            var medicine = await _medicineRepo.GetByIdAsync(dto.Id);
            if (medicine != null)
            {
                medicine.Name = dto.Name;
                medicine.Price = dto.Price;
                medicine.StockQuantity = dto.StockQuantity;
                medicine.Description = dto.Description;
                medicine.CategoryId = dto.CategoryId;

                await _medicineRepo.SaveChangesAsync();
            }
        }
    }
}