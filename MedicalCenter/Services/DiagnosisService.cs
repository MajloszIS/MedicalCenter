using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class DiagnosisService : IDiagnosisService
    {
        private readonly IDiagnosisRepository _diagnosisRepository;

        public DiagnosisService(IDiagnosisRepository diagnosisRepository)
        {
            _diagnosisRepository = diagnosisRepository;
        }

        public async Task<DiagnosisDto> GetByIdAsync(Guid id)
        {
            var diagnosis = await _diagnosisRepository.GetByIdAsync(id);
            if (diagnosis == null) return null;

            return new DiagnosisDto
            {
                Id = diagnosis.Id,
                MedicalRecordId = diagnosis.MedicalRecordId,
                Description = diagnosis.Description,
                Treatments = diagnosis.Treatments.Select(t => new TreatmentDto
                {
                    Id = t.Id,
                    DiagnosisId = diagnosis.Id,
                    Description = t.Description
                }).ToList()
            };
        }

        public async Task CreateDiagnosisAsync(DiagnosisDto diagnosisDto)
        {
            var diagnosis = new Diagnosis
            {
                MedicalRecordId = diagnosisDto.MedicalRecordId,
                Description = diagnosisDto.Description,
                Treatments = diagnosisDto.Treatments.Select(t => new Treatment
                {
                    Description = t.Description
                }).ToList()
            };

            await _diagnosisRepository.CreateDiagnosisAsync(diagnosis);
        }

        public async Task UpdateDiagnosisAsync(DiagnosisDto diagnosisDto)
        {
            var diagnosis = await _diagnosisRepository.GetByIdAsync(diagnosisDto.Id);
            if (diagnosis == null) return;

            diagnosis.Description = diagnosisDto.Description;

            await _diagnosisRepository.UpdateDiagnosisAsync(diagnosis);
        }

        public async Task DeleteDiagnosisAsync(Guid id)
        {
            await _diagnosisRepository.DeleteDiagnosisAsync(id);
        }
    }
}