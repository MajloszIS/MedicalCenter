using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly AppDbContext _context;

        public MedicineRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Medicine>> GetAllMedicinesAsync()
        {
            return _context.Medicines.Include(m => m.Category).ToListAsync();
        }

        public Task<Medicine?> GetByIdAsync(Guid id)
        {
            return _context.Medicines.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task AddMedicineAsync(Medicine medicine)
        {
            await _context.Medicines.AddAsync(medicine);
        }
        public async Task DeleteMedicineAsync(Medicine medicine)
        {
            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MedicineCategory>> GetAllCategoriesAsync()
        {
            return await _context.MedicineCategories.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddCategoryAsync(MedicineCategory category)
        {
            await _context.MedicineCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
        public async Task<MedicineCategory> GetCategoryByIdAsync(Guid id)
        {
            return await _context.MedicineCategories.FindAsync(id) ?? throw new Exception("Nie znaleziono kategorii leku.");
        }

        public async Task UpdateCategoryAsync(MedicineCategory category)
        {
            _context.MedicineCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(MedicineCategory category)
        {
            _context.MedicineCategories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasMedicinesInCategoryAsync(Guid categoryId)
        {
            return await _context.Medicines.AnyAsync(m => m.CategoryId == categoryId);
        }
    }
}