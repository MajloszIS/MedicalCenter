namespace MedicalCenter.DTOs
{
    public class CartItemDto
    {
        public Guid MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalItemPrice => Quantity * UnitPrice;
    }
}