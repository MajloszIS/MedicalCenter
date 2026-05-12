using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddCourierSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "Phone", "ProfilePicturePath", "RoleId" },
                values: new object[] { new Guid("ffffffff-1111-1111-1111-111111111111"), "kurier@medical.pl", "Szybki", "Wiesiek", "$2a$11$VmozT24fOt40zBIdkcNYFeO6z0sVfe2GdFOzyoSKVgSATzjNZSia6", "999888777", null, 4 });

            migrationBuilder.InsertData(
                table: "Couriers",
                columns: new[] { "Id", "UserId" },
                values: new object[] { new Guid("ffffffff-2222-2222-2222-222222222222"), new Guid("ffffffff-1111-1111-1111-111111111111") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Couriers",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-1111-1111-1111-111111111111"));
        }
    }
}
