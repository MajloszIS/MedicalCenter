// DTOs/ReserveAppointmentDto.cs
public class ReserveAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}