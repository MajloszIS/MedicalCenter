using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface IDepartmentRepository
    {
        public Task<List<Department>> GetAllDepartmentsAsync();
        public Task AddDepartmentAsync(Department department);
        public Task<bool> ExistsAsnyc(string departmentName);
    }
}
