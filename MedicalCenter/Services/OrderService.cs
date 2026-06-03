using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicalCenter.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderDto>> GetPatientOrdersAsync(Guid patientId)
        {
            var orders = await _orderRepository.GetOrdersByPatientIdAsync(patientId);
            var ratings = await _orderRepository.GetOrderRatingsMapAsync();
            return MapToDto(orders, ratings);
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync(int skip = 0, int take = int.MaxValue)
        {
            var orders = await _orderRepository.GetAllOrdersAsync(skip, take);
            var ratings = await _orderRepository.GetOrderRatingsMapAsync();
            return MapToDto(orders, ratings);
        }
        public async Task<int> GetOrdersCountAsync()
        {
            return await _orderRepository.GetOrdersCountAsync();
        }
        public async Task AddOrderRatingAsync(OrderRating rating)
        {
            await _orderRepository.AddOrderRatingAsync(rating);
        }

        private List<OrderDto> MapToDto(List<MedicalCenter.Models.Order> orders, Dictionary<Guid, OrderRating> ratings)
        {
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                StatusName = o.Status?.Name ?? TranslateStatusId(o.StatusId.ToString()),
                PatientFullName = o.Patient?.User != null ? $"{o.Patient.User.FirstName} {o.Patient.User.LastName}" : "Nieznany pacjent",
                Rating = ratings.TryGetValue(o.Id, out var r) ? r.Rating : null,
                RatingComment = ratings.TryGetValue(o.Id, out var rc) ? rc.Comment : null,
                Items = o.Items?.Select(i => new OrderItemDto
                {
                    MedicineName = i.Medicine?.Name ?? "Nieznany lek",
                    Quantity = i.Quantity
                }).ToList() ?? new List<OrderItemDto>()
            }).ToList();
        }

        private string TranslateStatusId(string statusIdStr)
        {
            if (statusIdStr == "1" || statusIdStr.Contains("11111111-1111")) return "Nowe";
            if (statusIdStr == "2" || statusIdStr.Contains("22222222-2222")) return "W realizacji";
            if (statusIdStr == "3" || statusIdStr.Contains("33333333-3333")) return "Wysłane";
            if (statusIdStr == "4" || statusIdStr.Contains("44444444-4444")) return "Zakończone";

            return "Weryfikacja płatności"; 
        }

        public async Task<InvoiceDto?> GetInvoiceByOrderIdAsync(Guid orderId)
        {
            var invoice = await _orderRepository.GetInvoiceByOrderIdAsync(orderId);
            if (invoice == null) return null;

            var address = invoice.Patient?.Address;
            string? addressLine1 = address != null
                ? $"{address.Street} {address.HouseNumber}" + (!string.IsNullOrEmpty(address.ApartmentNumber) ? $"/{address.ApartmentNumber}" : "")
                : null;
            string? addressLine2 = address != null
                ? $"{address.PostalCode} {address.City}"
                : null;

            return new InvoiceDto
            {
                Id = invoice.Id,
                OrderId = invoice.OrderId,
                IssuedAt = invoice.IssuedAt,
                PatientUserId = invoice.Patient?.UserId ?? Guid.Empty,
                PatientFullName = invoice.Patient?.User != null
                    ? $"{invoice.Patient.User.FirstName} {invoice.Patient.User.LastName}"
                    : "Nieznany pacjent",
                PatientIdStr = invoice.PatientId.ToString().Substring(0, 8),

                PatientAddressLine1 = addressLine1,
                PatientAddressLine2 = addressLine2,

                Amount = invoice.Amount,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                StripePaymentId = invoice.StripePaymentId,
                Items = invoice.Order?.Items?.Select(oi => new InvoiceItemDto
                {
                    MedicineName = oi.Medicine?.Name ?? "Nieznany lek",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<InvoiceItemDto>()
            };
        }
        public byte[] GenerateInvoicePdf(InvoiceDto invoice)
        {
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

            return document.GeneratePdf();
        }
    }
}