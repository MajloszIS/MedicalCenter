namespace MedicalCenter.DTOs
{
    public class TreatmentDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DiagnosisId { get; set; }

        public string Description { get; set; }
    }
}
