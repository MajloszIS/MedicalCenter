namespace MedicalCenter.DTOs
{
    public class SpecializationMonthlyReportDto
    {
        public int ReportYear { get; set; }
        public int ReportMonth { get; set; }
        public string SpecializationName { get; set; }
        public int ActiveDoctorsCount { get; set; }
        public int UniquePatientsCount { get; set; }
        public int ScheduledCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int PrescriptionsIssued { get; set; }
    }
}
