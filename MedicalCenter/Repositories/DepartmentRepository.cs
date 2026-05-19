using MedicalCenter.Controllers;
using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;
        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments.ToListAsync();
            return departments;
        }
        public async Task AddDepartmentAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsnyc(string departmentName)
        {
            return await _context.Departments.AnyAsync(d => d.Name == departmentName);

        }
    }
}
