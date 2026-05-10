namespace MedicalCenter.DTOs
{
    public class TreatmentDto
    {
        public Guid Id { get; set; }

        public Guid DiagnosisId { get; set; }

        public string Description { get; set; }
    }
}
