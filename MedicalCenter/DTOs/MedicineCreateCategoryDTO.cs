using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.DTOs
{
    public class MedicineCreateCategoryDTO
    {
        [Required(ErrorMessage = "Nazwa kategorii jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa kategorii nie może być dłuższa niż 100 znaków.")]
        [Display(Name = "Nazwa kategorii")]
        public string Name { get; set; } = null!;
    }
}