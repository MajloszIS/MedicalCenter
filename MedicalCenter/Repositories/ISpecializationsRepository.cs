using MedicalCenter.DTOs;
using MedicalCenter.Models;

namespace MedicalCenter.Repositories
{
    public interface ISpecializationsRepository
    {
        public Task<List<Specialization>> GetAllSpecializationsAsync();
        public Task<Specialization?> GetSpecializationByIdAsync(Guid id);
        public Task CreateSpecializationAsync(Specialization specialization);
        public Task UpdateSpecializationAsync(Specialization specialization);
        public Task DeleteSpecializationAsync(Guid id);
        public Task<Specialization?> GetSpecializationByNameAsync(string name);
        public Task<List<SpecializationMonthlyReportDto>> GetMonthlySpecializationReportAsync(int year, int month);

    }
}
