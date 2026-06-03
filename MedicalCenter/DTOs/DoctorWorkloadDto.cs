namespace MedicalCenter.DTOs
{
    public class DoctorWorkloadDto
    {
        public Guid DoctorId { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public int TotalAppointments { get; set; }
        public int UniquePatients { get; set; }
        public int ScheduledCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int PrescriptionsIssued { get; set; }
    }
}
