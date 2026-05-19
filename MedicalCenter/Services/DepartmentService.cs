using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Stripe;
using System.Linq.Expressions;

namespace MedicalCenter.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        public async Task<List<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllDepartmentsAsync();

            var deprtmentsDto = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                DoctorDepartments = d.DoctorDepartments,
            }).ToList();

            return deprtmentsDto;
        }
        public async Task AddDepartmentAsync(DepartmentDto departmentDto)
        {
            if (await _departmentRepository.ExistsAsnyc(departmentDto.Name))
            {
                throw new Exception("Departament o podanej nazwie już istnieje");
            }

            var departament = new Department
            {
                Name = departmentDto.Name,
            };
            await _departmentRepository.AddDepartmentAsync(departament);
        }
        public async Task<DepartmentDto> GetDepartmentByIdAsync(Guid departmentId)
        {
            var department = await _departmentRepository.GetDepartmentByIdAsync(departmentId);
            if (department == null)
            {
                throw new Exception("Nie znaleziono Departamentu o podanym ID");
            }

            var departamentDto = new DepartmentDto 
            { 
                Id = departmentId, 
                Name= department.Name,
                DoctorDepartments = department.DoctorDepartments,
            };

            return departamentDto;
        }
        public async Task DeleteDepartmentAsync(Guid departmentId)
        {
            var department = await _departmentRepository.GetDepartmentByIdAsync(departmentId);
            if (department == null)
            {
                throw new NullReferenceException("Nie znaleziono opinii o podanym ID.");
            }
            await _departmentRepository.DeleteDepartmentAsync(departmentId);
        }
    }
}
