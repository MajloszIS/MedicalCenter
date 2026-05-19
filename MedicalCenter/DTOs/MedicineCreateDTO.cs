using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.DTOs
{
    public class MedicineCreateDto
    {
        [Required(ErrorMessage = "Nazwa leku jest wymagana.")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana.")]
        [Range(0.01, 10000, ErrorMessage = "Cena musi być większa niż 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Ilość w magazynie jest wymagana.")]
        [Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna.")]
        public int StockQuantity { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Wybierz kategorię leku.")]
        public Guid CategoryId { get; set; }
    }
}