namespace MedicalCenter.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public DoctorDto Doctor { get; set; }
        public PatientDto Patient { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public string? Notes { get; set; }
    }
}
