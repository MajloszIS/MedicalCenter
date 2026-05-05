namespace MedicalCenter.DTOs
{
    public class DiagnosisDto
    {
        public Guid Id { get; set; }

        public Guid MedicalRecordId { get; set; }

        public string Description { get; set; }

        public List<TreatmentDto> Treatments { get; set; }
    }
}
