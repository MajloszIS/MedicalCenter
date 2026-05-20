using MedicalCenter.Models;
using System.Numerics;

namespace MedicalCenter.Repositories
{
    public interface IDepartmentRepository
    {
        public Task<List<Department>> GetAllDepartmentsAsync();
        public Task AddDepartmentAsync(Department department);
        public Task<bool> ExistsAsnyc(string departmentName);
        public Task<Department?> GetDepartmentByIdAsync(Guid departmentId);
        public Task DeleteDepartmentAsync(Guid departmentId);
        public Task EditDepartmentAsync(Department department);
        public Task<List<Department>> GetDepartmentsByIdsAsync(List<Guid> ids);
    }
}
