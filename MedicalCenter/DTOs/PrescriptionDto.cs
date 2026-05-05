namespace MedicalCenter.DTOs
{
    public class PrescriptionDto
    {
        public Guid Id { get; set; }

        public Guid MedicalRecordId { get; set; }
        public Guid DoctorId { get; set; }

        public List<PrescriptionItemDto> Items { get; set; }
    }
}
