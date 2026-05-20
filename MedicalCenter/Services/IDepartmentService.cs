using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Services
{
    public interface IDepartmentService
    {
        public Task<List<DepartmentDto>> GetAllDepartmentsAsync();
        public Task AddDepartmentAsync(DepartmentDto departmentDto);
        public Task<DepartmentDto> GetDepartmentByIdAsync(Guid departmentId);
        public Task DeleteDepartmentAsync(Guid departmentId);
        public Task EditDepartmentAsync(DepartmentDto departmentDto);
    }
}
