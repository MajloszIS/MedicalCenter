using MedicalCenter.Data;
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

        public Task<Medicine> GetByIdAsync(Guid id)
        {
            return _context.Medicines.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task AddMedicineAsync(Medicine medicine)
        {
            await _context.Medicines.AddAsync(medicine);
        }

        public async Task<List<MedicineCategory>> GetAllCategoriesAsync()
        {
            return await _context.MedicineCategories.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}