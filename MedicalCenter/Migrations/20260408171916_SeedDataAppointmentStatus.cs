using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataAppointmentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppointmentStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("12345678-1234-1234-1234-123456789012"), "Anulowana" },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000000"), "Zakończona" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Zaplanowana" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppointmentStatuses",
                keyColumn: "Id",
                keyValue: new Guid("12345678-1234-1234-1234-123456789012"));

            migrationBuilder.DeleteData(
                table: "AppointmentStatuses",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "AppointmentStatuses",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"));
        }
    }
}
