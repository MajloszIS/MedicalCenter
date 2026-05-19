using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface IDepartmentService
    {
        public Task<List<DepartmentDto>> GetAllDepartmentsAsync();
        public Task AddDepartmentAsync(DepartmentDto departmentDto);
    }
}
