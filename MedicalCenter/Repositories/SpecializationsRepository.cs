using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class SpecializationsRepository : ISpecializationsRepository
    {
        private readonly AppDbContext _context;
        public SpecializationsRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<List<Specialization>> GetAllSpecializationsAsync()
        {
            return _context.Specializations.ToListAsync();
        }
        public Task<Specialization> GetSpecializationByIdAsync(Guid id)
        {
            return _context.Specializations.FirstOrDefaultAsync(s => s.Id == id);
        }
        public Task CreateSpecializationAsync(Specialization specialization)
        {
            _context.Specializations.Add(specialization);
            return _context.SaveChangesAsync();
        }
        public Task UpdateSpecializationAsync(Specialization specialization)
        {
            _context.Specializations.Update(specialization);
            return _context.SaveChangesAsync();
        }
        public async Task DeleteSpecializationAsync(Guid id)
        {
            var specialization = await _context.Specializations.FirstOrDefaultAsync(s => s.Id == id);
            if (specialization != null)
            {
                _context.Specializations.Remove(specialization);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Specialization> GetSpecializationByNameAsync(string name)
        {
            return await _context.Specializations.FirstOrDefaultAsync(s => s.Name == name);
        }
    }
}
