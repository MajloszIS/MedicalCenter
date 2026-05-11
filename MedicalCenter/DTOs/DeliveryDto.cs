namespace MedicalCenter.DTOs
{
    public class DeliveryDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        // Dane pacjenta i adresu (z order i address)
        public string PatientFullName { get; set; }
        public string DeliveryAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        // Informacje o zamówieniu
        public decimal TotalPrice { get; set; }
        public string StatusName { get; set; }
    }
}