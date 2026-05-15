using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                Doctor = new DoctorDto
                {
                    Id = p.Doctor.Id,
                    FirstName = p.Doctor.User.FirstName,
                    LastName = p.Doctor.User.LastName,
                    Phone = p.Doctor.User.Phone,
                    SpecializationName = p.Doctor.Specialization.Name
                },
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

        public async Task<byte[]> GeneratePrescriptionPdfAsync(Guid prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
            if (prescription == null)
                throw new Exception("Prescription not found");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(14, 20, Unit.Centimetre);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Recepta")
                        .FontSize(20).Bold();

                    page.Content().Column(column =>
                    {
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        column.Spacing(10);
                        column.Item().Text($"Pacjent: {prescription.MedicalRecord.Patient.User.FirstName} {prescription.MedicalRecord.Patient.User.LastName}");
                        column.Item().Text($"Wystawca: Lekarz {prescription.Doctor.User.FirstName} {prescription.Doctor.User.LastName}");
                        column.Item().Text($"Numer licencji: {prescription.Doctor.LicenseNumber}");
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        column.Spacing(10);
                        column.Item().Text($"Numer recepty: {prescription.Id}");
                        column.Item().Text($"Liczba leków: {prescription.Items.Count}");

                        foreach (var item in prescription.Items)
                        {
                            column.Item().Text($"- {item.Medicine.Name}, ilość: {item.Quantity}");
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Centrum Medyczne");
                });
            });

            return document.GeneratePdf();
        }

    }
}
