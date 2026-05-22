using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface IMedicineService
    {
        Task<List<MedicineDto>> GetAllMedicineAsync();
        Task<List<MedicineDto>> GetAvailableMedicinesAsync();
        Task AddMedicineAsync(MedicineCreateDto dto);
        Task DeleteMedicineAsync(Guid id);
        Task<List<MedicineCategory>> GetAllCategoriesAsync();
        Task<UpdateMedicineDto> GetMedicineForEditAsync(Guid id);
        Task UpdateMedicineAsync(UpdateMedicineDto dto);
        Task AddCategoryAsync(MedicineCreateCategoryDTO dto);
        Task UpdateCategoryAsync(Guid id, string name);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}