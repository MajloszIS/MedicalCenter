using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class PharmacyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MedicineCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("cccccccc-1111-1111-1111-111111111111"), "Leki przeciwbólowe" },
                    { new Guid("cccccccc-2222-2222-2222-222222222222"), "Syrop" },
                    { new Guid("cccccccc-3333-3333-3333-333333333333"), "Leki" }
                });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-1111-1111-1111-111111111111"), "Nowe" },
                    { new Guid("bbbbbbbb-2222-2222-2222-222222222222"), "W realizacji" },
                    { new Guid("bbbbbbbb-3333-3333-3333-333333333333"), "Wysłane" },
                    { new Guid("bbbbbbbb-4444-4444-4444-444444444444"), "Zakończone" }
                });

            migrationBuilder.InsertData(
                table: "Medicines",
                columns: new[] { "Id", "CategoryId", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("dddddddd-1111-1111-1111-111111111111"), new Guid("cccccccc-1111-1111-1111-111111111111"), "Apap Extra", 15.50m },
                    { new Guid("dddddddd-2222-2222-2222-222222222222"), new Guid("cccccccc-1111-1111-1111-111111111111"), "Ibuprom Max", 12.99m },
                    { new Guid("dddddddd-3333-3333-3333-333333333333"), new Guid("cccccccc-3333-3333-3333-333333333333"), "Rutinoscorbin", 9.00m },
                    { new Guid("dddddddd-4444-4444-4444-444444444444"), new Guid("cccccccc-2222-2222-2222-222222222222"), "Syrop na kaszel", 21.30m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-3333-3333-3333-333333333333"));
        }
    }
}
