namespace MedicalCenter.DTOs
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalCartPrice => Items.Sum(i => i.TotalItemPrice);
    }
}