using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IMedicineRepository
    {
        Task<List<Medicine>> GetAllMedicinesAsync();
        Task<Medicine> GetByIdAsync(Guid id);
        Task AddMedicineAsync(Medicine medicine);
        Task DeleteMedicineAsync(Medicine medicine);
        Task<List<MedicineCategory>> GetAllCategoriesAsync();
        Task SaveChangesAsync();
        Task AddCategoryAsync(MedicineCategory category);
        Task<MedicineCategory> GetCategoryByIdAsync(Guid id);
        Task UpdateCategoryAsync(MedicineCategory category);
        Task DeleteCategoryAsync(MedicineCategory category);
        Task<bool> HasMedicinesInCategoryAsync(Guid categoryId);
    }
}