using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedure_AppointmentReserve : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = File.ReadAllText(
                Path.Combine("DbScripts", "Procedures", "usp_AppointmentReserve.sql"));
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.usp_AppointmentReserve;");
        }
    }
}
