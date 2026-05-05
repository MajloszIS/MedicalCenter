namespace MedicalCenter.DTOs
{
    public class PrescriptionItemDto
    {
        public Guid Id { get; set; }

        public Guid PrescriptionId { get; set; }
        public Guid MedicineId { get; set; }

        public int Quantity { get; set; }

        public MedicineDto Medicine { get; set; }
    }
}
