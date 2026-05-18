using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationsRepository _specializationRepository;
        public SpecializationService(ISpecializationsRepository specializationRepository) 
        {
            _specializationRepository = specializationRepository;
        }

        public async Task<List<SpecializationDto>> GetAllSpecializationsAsync()
        {
            var specializations = await _specializationRepository.GetAllSpecializationsAsync();
            var specializationDtos = specializations.Select(s => new SpecializationDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
            return specializationDtos;
        }
        public async Task<SpecializationDto> GetSpecializationByIdAsync(Guid id)
        {
            var specialization = await _specializationRepository.GetSpecializationByIdAsync(id);
            if (specialization == null)
            {
                return null;
            }
            return new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name
            };
        }
        public async Task CreateSpecializationAsync(SpecializationDto specializationDto)
        {
            var specializations = await _specializationRepository.GetAllSpecializationsAsync();

            if(specializations.Any(s => s.Name == specializationDto.Name))
            {
                throw new InvalidOperationException("Specjalizacja o tej nazwie już istnieje.");
            }

            var specialization = new Specialization
            {
                Name = specializationDto.Name
            };
            await _specializationRepository.CreateSpecializationAsync(specialization);
        }
        public async Task UpdateSpecializationAsync(Guid id, SpecializationDto specializationDto)
        {
            var specialization = await _specializationRepository.GetSpecializationByIdAsync(id);
            if (specialization == null)
            {
                throw new InvalidOperationException("Specjalizacja o podanym ID nie istnieje.");
            }
            var specializations = await _specializationRepository.GetAllSpecializationsAsync();

            if (specializations.Any(s => s.Name == specializationDto.Name))
            {
                throw new InvalidOperationException("Specjalizacja o tej nazwie już istnieje.");
            }
            specialization.Name = specializationDto.Name;
            await _specializationRepository.UpdateSpecializationAsync(specialization);
        }
        public async Task DeleteSpecializationAsync(Guid id)
        {
            var specialization = await _specializationRepository.GetSpecializationByIdAsync(id);
            if (specialization == null)
            {
                return;
            }
            await _specializationRepository.DeleteSpecializationAsync(id);
        }
    }
}
