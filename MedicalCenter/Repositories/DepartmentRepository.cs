using MedicalCenter.Controllers;
using MedicalCenter.Data;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;

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
        public async Task<Department?> GetDepartmentByIdAsync(Guid departmentId)
        {
            var department = await _context.Departments
                .Where(r => r.Id == departmentId)
                .FirstOrDefaultAsync();
            return department;
        }
        public async Task DeleteDepartmentAsync(Guid departmentId)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }

    }
}
