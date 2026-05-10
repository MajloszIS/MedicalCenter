using MedicalCenter.DTOs;
using MedicalCenter.Repositories;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        public PrescriptionService(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }
        public async Task<PrescriptionDto> GetByIdAsync(Guid id)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new Exception("Prescription not found");
            }

            var prescriptionDto = new PrescriptionDto
            {
                Id = prescription.Id,
                MedicalRecordId = prescription.MedicalRecordId,
                DoctorId = prescription.DoctorId,
                Items = prescription.Items.Select(i => new PrescriptionItemDto
                {
                    Id = i.Id,
                    PrescriptionId = i.PrescriptionId,
                    Quantity = i.Quantity,
                }).ToList()
            };

            return prescriptionDto;
        }
        public async Task CreatePrescription(PrescriptionDto prescriptionDto)
        {
            await _prescriptionRepository.CreatePrescriptionAsync(new Prescription
            {  
                MedicalRecordId = prescriptionDto.MedicalRecordId,
                DoctorId = prescriptionDto.DoctorId,
                Items = prescriptionDto.Items.Select(i => new PrescriptionItem
                {
                    Id = i.Id,
                    MedicineId = i.MedicineId,
                    PrescriptionId = i.PrescriptionId,
                    Quantity = i.Quantity,
                }).ToList()
            });

        }
        public async Task UpdatePrescription(PrescriptionDto prescriptionDto)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionDto.Id);
            if (prescription == null)
            {
                throw new Exception("Prescription not found");
            }
            prescription.Items = prescriptionDto.Items.Select(i => new PrescriptionItem
            {
                Id = i.Id,
                PrescriptionId = i.PrescriptionId,
                Quantity = i.Quantity,
            }).ToList();
            await _prescriptionRepository.UpdatePrescriptionAsync(prescription);
        }
        public async Task DeletePrescription(Guid id)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new Exception("Prescription not found");
            }
            else
            {
                await _prescriptionRepository.DeletePrescriptionAsync(prescription);
            }
        }
        public async Task DeletePrescriptionItemAsync(Guid prescriptionItemId)
        {
            var prescriptionItem = await _prescriptionRepository.GetPrescriptionItemByIdAsync(prescriptionItemId);
            if (prescriptionItem == null)
            {
                throw new Exception("Prescription not found");
            }
            else
            {
                await _prescriptionRepository.DeletePrescriptionItemAsync(prescriptionItem);
            }
        }
        public async Task<List<PrescriptionDto>> GetPrescriptionsByPatientIdAsync(Guid patientId)
        {
            var prescriptions = await _prescriptionRepository.GetPrescriptionsByPatientIdAsync(patientId);
            return prescriptions.Select(p => new PrescriptionDto
            {
                Id = p.Id,
                MedicalRecordId = p.MedicalRecordId,
                DoctorId = p.DoctorId,
                Items = p.Items.Select(i => new PrescriptionItemDto
                {
                    Id = i.Id,
                    PrescriptionId = i.PrescriptionId,
                    Quantity = i.Quantity,
                    Medicine = new MedicineDto
                    {
                        Id = i.Medicine.Id,
                        Name = i.Medicine.Name,
                        Price = i.Medicine.Price
                    }
                }).ToList()
            }).ToList();
        }

    }
}
