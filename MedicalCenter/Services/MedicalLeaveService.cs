using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicalCenter.Services
{
    public class MedicalLeaveService : IMedicalLeaveService
    {
        private readonly IMedicalLeaveRepository _medicalLeaveRepository;
        public MedicalLeaveService(IMedicalLeaveRepository medicalLeaveRepository) 
        {
            _medicalLeaveRepository = medicalLeaveRepository;
        }
        public async Task CreateMedicalLeaveAsync(MedicalLeaveDto medicalLeaveDto)
        {
            if (medicalLeaveDto == null)
                throw new ArgumentNullException("Podano puste dane", nameof(medicalLeaveDto));

            if (medicalLeaveDto.DateTo <= medicalLeaveDto.DateFrom)
                throw new ArgumentException("Data końca musi być późniejsza niż data początku");

            var medicalLeave = new MedicalLeave
            {
                DoctorId = medicalLeaveDto.DoctorId,
                PatientId = medicalLeaveDto.PatientId,
                DateFrom = medicalLeaveDto.DateFrom,
                DateTo = medicalLeaveDto.DateTo,
                Reason = medicalLeaveDto.Reason,
                IssuedAt = DateTime.UtcNow
            };

            await _medicalLeaveRepository.AddMedicalLeaveAsync(medicalLeave);
        }
        public async Task<List<MedicalLeaveDto>> GetMedicalLeavesByPatientIdAsync(Guid patientId)
        {
            var medicalLeaves = await _medicalLeaveRepository.GetMedicalLeavesByPatientIdAsync(patientId);
            if (medicalLeaves == null || !medicalLeaves.Any())
                throw new Exception("Nie znaleziono zwolnień");

            var medicalLeavesDto = medicalLeaves.Select(ml => new MedicalLeaveDto
            { 
                Id = ml.Id, 
                DateFrom = ml.DateFrom,
                DateTo = ml.DateTo,
                DoctorId = ml.DoctorId,
                PatientId = ml.PatientId,
                IssuedAt = ml.IssuedAt,
                Reason = ml.Reason,
                DoctorFulLName = ml.Doctor.User.FirstName + " " + ml.Doctor.User.LastName,
                PatientFullName = ml.Patient.User.FirstName + " " + ml.Patient.User.LastName
            }).ToList();

            return medicalLeavesDto;
        }
        public async Task<byte[]> GenerateMedicalLeavePdfAsync(Guid id)
        {
            var medicalLeave = await _medicalLeaveRepository.GetMedicalLeaveIdAsync(id);
            if (medicalLeave == null)
                throw new Exception("Nie znaleziono Zwolnienia");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Nagłówek faktury
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("MedicalCenter").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                            col.Item().Text("ul. Wiejska 1, Białystok");
                            col.Item().Text("NIP: 123-456-78-90");
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("Zwolnienie").FontSize(20).SemiBold();
                            col.Item().Text($"Wystawione: {medicalLeave.IssuedAt.ToLocalTime():dd.MM.yyyy}");
                        });
                    });

                    // Treść faktury
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        column.Spacing(10);
                        column.Item().Text($"Pacjent: {medicalLeave.Patient.User.FirstName} {medicalLeave.Patient.User.LastName}");
                        column.Item().Text($"Wystawca: Lekarz {medicalLeave.Doctor.User.FirstName} {medicalLeave.Doctor.User.LastName}");
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        column.Spacing(10);
                        column.Item().Text($"Od: {medicalLeave.DateFrom.ToString("dd.MM.yyyy")}");
                        column.Item().Text($"Do: {medicalLeave.DateTo.ToString("dd.MM.yyyy")}");
                        column.Item().Text($"Powód: {medicalLeave.Reason}");
                    });

              

                    // Stopka
                    page.Footer().AlignCenter().Text($"Wygenerowano automatycznie w systemie MedicalCenter").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }
    }
}
