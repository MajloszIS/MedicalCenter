namespace MedicalCenter.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }
        public string StatusName { get; set; }
        public string PatientFullName { get; set; } 
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public int? Rating { get; set; }
        public string? RatingComment { get; set; }
    }

    public class OrderItemDto
    {
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}