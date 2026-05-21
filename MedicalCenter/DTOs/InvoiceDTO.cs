using System;
using System.Collections.Generic;

namespace MedicalCenter.DTOs
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string OrderNumber => OrderId.ToString().Substring(0, 8).ToUpper();
        public DateTime IssuedAt { get; set; }
        public Guid PatientUserId { get; set; }
        public string PatientFullName { get; set; } = null!;
        public string PatientIdStr { get; set; } = null!;
        public string? PatientAddressLine1 { get; set; }
        public string? PatientAddressLine2 { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string StripePaymentId { get; set; } = null!;
        public List<InvoiceItemDto> Items { get; set; } = new();
    }

    public class InvoiceItemDto
    {
        public string MedicineName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}