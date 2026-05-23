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
            var medicalRecordDto = new MedicalRecordDto
            {
                Id = medicalRecord.Id,
                PatientId = medicalRecord.PatientId,
                DoctorId = medicalRecord.DoctorId,

                Patient = new PatientDto
                {
                    Id = medicalRecord.Patient.Id,
                    FirstName = medicalRecord.Patient.User.FirstName,
                    LastName = medicalRecord.Patient.User.LastName,
                    Phone = medicalRecord.Patient.User.Phone,
                    Pesel = medicalRecord.Patient.Pesel
                },

                Diagnoses = medicalRecord.Diagnoses.Select(d => new DiagnosisDto
                {
                    Id = d.Id,
                    MedicalRecordId = d.MedicalRecordId,
                    Description = d.Description,
                    DiagnosedAt = d.DiagnosedAt,
                    Treatments = d.Treatments.Select(t => new TreatmentDto
                    {
                        Id = t.Id,
                        DiagnosisId = t.DiagnosisId,
                        Description = t.Description
                    }).ToList()
                }).ToList(),

                Prescriptions = medicalRecord.Prescriptions.Select(p => new PrescriptionDto
                {
                    Id = p.Id,
                    MedicalRecordId = p.MedicalRecordId,
                    DoctorId = p.DoctorId,
                    IssuedAt = p.IssuedAt,
                    Items = p.Items.Select(i => new PrescriptionItemDto
                    {
                        Id = i.Id,
                        PrescriptionId = i.PrescriptionId,
                        MedicineId = i.MedicineId,
                        Quantity = i.Quantity,
                        Notes = i.Notes,
                        Medicine = new MedicineDto
                        {
                            Id = i.Medicine.Id,
                            Name = i.Medicine.Name,
                            Price = i.Medicine.Price
                        }
                    }).ToList()
                }).ToList()

            };

            return medicalRecordDto;
        }

        public async Task<MedicalRecordDto> GetOrCreateAsync(Guid doctorId, Guid patientId)
        {
            var medicalRecord = await _medicalRecordRepository.GetByDoctorAndPatientAsync(doctorId, patientId);

            if (medicalRecord == null)
            {
                var newMedicalRecord = new MedicalRecord
                {
                    DoctorId = doctorId,
                    PatientId = patientId
                };
                await _medicalRecordRepository.CreateMedicalRecordAsync(newMedicalRecord);
                medicalRecord = await _medicalRecordRepository.GetMedicalRecordByIdAsync(newMedicalRecord.Id)
                    ?? throw new Exception("Nie znaleziono utworzonej karty medycznej.");
            }

            return MapToDto(medicalRecord);

        }
        public async Task<MedicalRecordDto> GetMedicalRecordByIdAsync(Guid id)
        {
            var medicalRecord = await _medicalRecordRepository.GetMedicalRecordByIdAsync(id);
            if (medicalRecord == null)
            {
                throw new Exception("Nie znaleziono karty medycznej.");
            }
            else
            {
                return MapToDto(medicalRecord);

            }
        }
    }
}
