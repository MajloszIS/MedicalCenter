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
    }
}