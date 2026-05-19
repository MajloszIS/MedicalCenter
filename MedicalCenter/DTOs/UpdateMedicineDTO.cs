using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.DTOs
{
    public class UpdateMedicineDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public string? CategoryName { get; set; }
    }
}