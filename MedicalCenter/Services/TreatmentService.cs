using MedicalCenter.Models;
using MedicalCenter.DTOs;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly ITreatmentRepository _treatmentRepository;
        public TreatmentService(ITreatmentRepository treatmentRepository)
        {
            _treatmentRepository = treatmentRepository;
        }
        public async Task CreateTreatmentAsync(TreatmentDto treatmentDto)
        {
            var newTreatment = new Treatment
            {
                DiagnosisId = treatmentDto.DiagnosisId,
                Description = treatmentDto.Description
            };
            await _treatmentRepository.AddTreatmentAsync(newTreatment);
        }
        public async Task<TreatmentDto> GetTreatmentByIdAsync(Guid id)
        {
            var treatment = await _treatmentRepository.GetTreatmentByIdAsync(id);
            var treatmentDto = new TreatmentDto
            {
                Id = treatment.Id,
                DiagnosisId = treatment.DiagnosisId,
                Description = treatment.Description
            }; 
            return treatmentDto;
        }
        public async Task<List<TreatmentDto>> GetTreatmentsByDiagnosisIdAsync(Guid diagnosisId)
        {
            var treatments = await _treatmentRepository.GetTreatmentsByDiagnosisIdAsync(diagnosisId);
            var treatmentDtos = treatments.Select(t => new TreatmentDto
            {
                Id = t.Id,
                DiagnosisId = t.DiagnosisId,
                Description = t.Description
            }).ToList();
            return treatmentDtos;
        }
        public async Task UpdateTreatmentAsync(TreatmentDto treatmentDto)
        {
            var treatment = await _treatmentRepository.GetTreatmentByIdAsync(treatmentDto.Id);
            if (treatment == null)
            {
                await CreateTreatmentAsync(treatmentDto);
                return;
            }
            else
            {
                treatment.Description = treatmentDto.Description;
            }        
            await _treatmentRepository.UpdateTreatmentAsync(treatment);
        }
        public async Task DeleteTreatmentAsync(Guid treatmentId)
        {
            await _treatmentRepository.DeleteTreatmentAsync(treatmentId);
        }
    }
}
