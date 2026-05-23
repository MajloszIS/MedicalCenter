using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace MedicalCenter.Services
{
    public class ReferralService : IReferralService
    {
        private readonly IReferralRepository _referralRepository;
        public ReferralService(IReferralRepository referralRepository) 
        {
            _referralRepository = referralRepository;
        }
        public async Task CreateReferralAsync(ReferralDto referralDto)
        {
            if (referralDto == null)
                throw new ArgumentNullException("Podano puste dane", nameof(referralDto));

            var referral = new Referral
            {
                DoctorId = referralDto.DoctorId,
                PatientId = referralDto.PatientId,  
                TargetSpecialization = referralDto.TargetSpecialization,
                Description = referralDto.Description,
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30), // skierowanie ważne 30 dni
            };

            await _referralRepository.AddReferralAsync(referral);
        }
        public async Task<List<ReferralDto>> GetReferralsByPatientIdAsync(Guid patientId)
        {
            var referrals = await _referralRepository.GetReferralsByPatientIdAsync(patientId);

            if (referrals == null)
                throw new Exception("Nie znaleziono zwolnień");

            var referralsDto = referrals.Select(r => new ReferralDto
            {
                Id = r.Id, 
                DoctorId = r.DoctorId,
                PatientId = r.PatientId,
                TargetSpecialization = r.TargetSpecialization,
                Description= r.Description,
                IssuedDate= r.IssuedDate,
                ExpiryDate = r.ExpiryDate,
                DoctorFullName = r.Doctor.User.FirstName + " " + r.Doctor.User.LastName,
                PatientFullName = r.Patient.User.FirstName + " " + r.Patient.User.LastName
            }).ToList();

            return referralsDto;
        }
        public async Task<byte[]> GenerateReferralPdfAsync(Guid id)
        {
            var referral = await _referralRepository.GetMedicalLeaveIdAsync(id);
            if (referral == null)
                throw new Exception("Nie znaleziono Skierowania");

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
                            col.Item().Text("Skierowanie").FontSize(20).SemiBold();
                            col.Item().Text($"Wystawione: {referral.IssuedDate.ToLocalTime():dd.MM.yyyy}");
                        });
                    });

                    // Treść faktury
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        column.Spacing(10);
                        column.Item().Text($"Pacjent: {referral.Patient.User.FirstName} {referral.Patient.User.LastName}");
                        column.Item().Text($"Wystawca: Lekarz {referral.Doctor.User.FirstName} {referral.Doctor.User.LastName}");
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        column.Spacing(10);
                        column.Item().Text($"Specjalista: {referral.TargetSpecialization}");
                        column.Item().Text($"Opis: {referral.Description}");
                    });



                    // Stopka
                    page.Footer().AlignCenter().Text($"Wygenerowano automatycznie w systemie MedicalCenter").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }
    }
}
