using MedicalCenter.DTOs;

namespace MedicalCenter.Services
{
    public interface ISpecializationService
    {
        public Task<List<SpecializationDto>> GetAllSpecializationsAsync();
        public Task<SpecializationDto> GetSpecializationByIdAsync(Guid id);
        public Task CreateSpecializationAsync(SpecializationDto specializationDto);
        public Task UpdateSpecializationAsync(Guid id, SpecializationDto specializationDto);
        public Task DeleteSpecializationAsync(Guid id);
    }
}
