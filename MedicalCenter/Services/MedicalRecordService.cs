using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository) 
        {
            _medicalRecordRepository = medicalRecordRepository;
        }

        private MedicalRecordDto MapToDto(MedicalRecord medicalRecord)
        {
            return new MedicalRecordDto
            {
                Id = medicalRecord.Id,
                PatientId = medicalRecord.PatientId,
                DoctorId = medicalRecord.DoctorId,
                Date = medicalRecord.Date,
                Diagnoses = medicalRecord.Diagnoses?.Select(d => new DiagnosisDto
                {
                    Id = d.Id,
                    MedicalRecordId = d.MedicalRecordId,
                    Description = d.Description,
                    Treatments = d.Treatments?.Select(t => new TreatmentDto
                    {
                        Id = t.Id,
                        DiagnosisId = t.DiagnosisId,
                        Description = t.Description
                    }).ToList()
                }).ToList(),
                Prescriptions = medicalRecord.Prescriptions?.Select(p => new PrescriptionDto
                {
                    Id = p.Id,
                    MedicalRecordId = p.MedicalRecordId,
                    DoctorId = p.DoctorId,
                    Items = p.Items?.Select(i => new PrescriptionItemDto
                    {
                        Id = i.Id,
                        PrescriptionId = i.PrescriptionId,
                        MedicineId = i.MedicineId,
                        Quantity = i.Quantity,
                        Medicine = i.Medicine != null ? new MedicineDto
                        {
                            Id = i.Medicine.Id,
                            Name = i.Medicine.Name,
                            Price = i.Medicine.Price
                        } : null
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<MedicalRecordDto> GetOrCreateAsync(Guid doctorId, Guid patientId)
        {
            var medicalRecord = await _medicalRecordRepository.GetByDoctorAndPatientAsync(doctorId, patientId);
            if (medicalRecord == null)
            {
                var newMedicalRecord = new MedicalRecord
                {
                    DoctorId = doctorId,
                    PatientId = patientId,
                    Date = DateTime.Now
                };
                await _medicalRecordRepository.CreateMedicalRecordAsync(newMedicalRecord);
                medicalRecord = await _medicalRecordRepository.GetMedicalRecordByIdAsync(newMedicalRecord.Id);
            }

            return MapToDto(medicalRecord);

        }
        public async Task<MedicalRecordDto> GetMedicalRecordByIdAsync(Guid id)
        {
            var medicalRecord = await _medicalRecordRepository.GetMedicalRecordByIdAsync(id);
            if (medicalRecord == null)
            {
                throw new Exception("Medical record not found");
            }
            else
            {
                return MapToDto(medicalRecord);

            }
        }
    }
}
