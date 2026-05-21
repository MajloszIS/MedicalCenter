using MedicalCenter.Data;
using MedicalCenter.Repositories;
using MedicalCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicalCenter.Controllers
{
    [Authorize(Roles = "Patient")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPatientRepository _patientRepository;
        private readonly AppDbContext _context;

        public OrderController(
            IOrderService orderService,
            IPatientRepository patientRepository,
            AppDbContext context)
        {
            _orderService = orderService;
            _patientRepository = patientRepository;
            _context = context;
        }

        public async Task<IActionResult> MyOrders()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge();

            var patient = await _patientRepository.GetPatientByUserIdAsync(Guid.Parse(userIdStr));
            if (patient == null) return NotFound("Nie znaleziono pacjenta.");

            var orders = await _orderService.GetPatientOrdersAsync(patient.Id);
            return View(orders);
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> DownloadInvoice(Guid orderId)
        {
            var invoice = await _orderService.GetInvoiceByOrderIdAsync(orderId);

            if (invoice == null) return NotFound("Faktura nie została jeszcze wygenerowana.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (invoice.PatientUserId.ToString() != currentUserId) return Unauthorized();

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
                            col.Item().Text("FAKTURA VAT").FontSize(20).SemiBold();
                            col.Item().Text($"Data: {invoice.IssuedAt.ToLocalTime():dd.MM.yyyy HH:mm}");
                            col.Item().Text($"Zamówienie: #{invoice.OrderNumber}");
                        });
                    });

                    // Treść faktury
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        column.Item().PaddingBottom(20).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Nabywca:").SemiBold();
                                col.Item().Text(invoice.PatientFullName);

                                if (!string.IsNullOrEmpty(invoice.PatientAddressLine1))
                                {
                                    col.Item().Text(invoice.PatientAddressLine1);
                                    col.Item().Text(invoice.PatientAddressLine2);
                                }
                            });
                    });

                        // Tabela z lekami
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(1).Padding(2).Text("Nazwa produktu").SemiBold();
                                header.Cell().BorderBottom(1).Padding(2).AlignRight().Text("Ilość").SemiBold();
                                header.Cell().BorderBottom(1).Padding(2).AlignRight().Text("Cena").SemiBold();
                            });

                            foreach (var item in invoice.Items)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.MedicineName);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignRight().Text(item.Quantity.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignRight().Text(item.UnitPrice.ToString("C", new System.Globalization.CultureInfo("pl-PL")));
                            }
                        });

                        // Podsumowanie kosztów
                        column.Item().PaddingTop(20).AlignRight().Column(col =>
                        {
                            col.Item().Text($"Kwota netto: {invoice.Amount.ToString("C", new System.Globalization.CultureInfo("pl-PL"))}");
                            col.Item().Text($"Podatek VAT (8%): {invoice.TaxAmount.ToString("C", new System.Globalization.CultureInfo("pl-PL"))}");
                            col.Item().Text($"DO ZAPŁATY: {invoice.TotalAmount.ToString("C", new System.Globalization.CultureInfo("pl-PL"))}").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                        });
                    });

                    // Stopka
                    page.Footer().AlignCenter().Text($"Wygenerowano automatycznie w systemie MedicalCenter • Płatność Stripe ID: {invoice.StripePaymentId}").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            string fileName = $"Faktura_{invoice.OrderNumber}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}