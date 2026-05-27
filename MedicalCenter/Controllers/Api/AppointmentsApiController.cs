using MedicalCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("api/appointments")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class AppointmentsApiController : ControllerBase
{
    private readonly AppDbContext _context;
    public AppointmentsApiController(AppDbContext context) => _context = context;

    [HttpPost("reserve")]
    public async Task<IActionResult> Reserve([FromBody] ReserveAppointmentDto dto)
    {
        // parametry wejściowe
        var patientId = new SqlParameter("@PatientId", SqlDbType.UniqueIdentifier)
        { Value = dto.PatientId };
        var doctorId = new SqlParameter("@DoctorId", SqlDbType.UniqueIdentifier)
        { Value = dto.DoctorId };
        var appointmentDate = new SqlParameter("@AppointmentDate", SqlDbType.DateTime2)
        { Value = dto.AppointmentDate };
        var description = new SqlParameter("@Description", SqlDbType.NVarChar, -1)
        { Value = (object?)dto.Description ?? string.Empty };
        var notes = new SqlParameter("@Notes", SqlDbType.NVarChar, -1)
        { Value = (object?)dto.Notes ?? string.Empty };

        // parametr OUTPUT — kluczowe: Direction = Output
        var appointmentId = new SqlParameter("@AppointmentId", SqlDbType.UniqueIdentifier)
        { Direction = ParameterDirection.Output };

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"EXEC dbo.usp_AppointmentReserve 
                    @PatientId, @DoctorId, @AppointmentDate, 
                    @Description, @Notes, @AppointmentId OUTPUT",
                patientId, doctorId, appointmentDate, description, notes, appointmentId);

            // odczyt wartości OUTPUT — dopiero PO wykonaniu
            var newId = (Guid)appointmentId.Value;

            return Ok(new { appointmentId = newId, message = "Wizyta zarezerwowana." });
        }
        catch (SqlException ex)
        {
            // błędy z THROW 5000X łapiemy i zwracamy jako 400
            return BadRequest(new { error = ex.Message });
        }
    }
    [HttpPost("cancel-unpaid")]
    public async Task<IActionResult> CancelUnpaidOrders()
    {
        // Parametr OUTPUT
        var cancelledCount = new SqlParameter("@CancelledCount", SqlDbType.Int)
        { Direction = ParameterDirection.Output };

        try
        {
            // Wywołanie procedury z dedykowanego schematu shop przy użyciu surowego SQL
            await _context.Database.ExecuteSqlRawAsync(
                @"EXEC dbo.usp_CancelUnpaidOrders 
                    @CancelledCount OUTPUT",
                cancelledCount);

            // Odczyt liczby zmodyfikowanych rekordów zwróconej przez kursor w bazie
            var count = (int)cancelledCount.Value;

            return Ok(new
            {
                cancelledOrdersCount = count,
                message = $"Proces czyszczenia bazy zakończony. Pomyślnie anulowano przeterminowane zamówienia: {count} szt."
            });
        }
        catch (SqlException ex)
        {
            // Przechwytywanie ewentualnych błędów wykonania zapytania po stronie MS SQL
            return BadRequest(new { error = ex.Message });
        }
    }
}