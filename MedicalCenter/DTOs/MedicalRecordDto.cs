using MedicalCenter.Models;

namespace MedicalCenter.DTOs
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }

        public DateTime Date { get; set; }

        public List<DiagnosisDto> Diagnoses { get; set; }
        public List<PrescriptionDto> Prescriptions { get; set; }

        public PatientDto Patient { get; set; }

    }
}
