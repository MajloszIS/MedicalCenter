using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("55555555-3333-3333-3333-333333333333"));

            migrationBuilder.UpdateData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-3333-3333-3333-333333333333"),
                columns: new[] { "CategoryId", "Description", "Name", "Price", "StockQuantity" },
                values: new object[] { new Guid("aaaaaaaa-1111-1111-1111-111111111111"), "Silny lek przeciwbólowy", "Maść na Ból Dupy", 999999.99m, 1 });

            migrationBuilder.UpdateData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-4444-4444-4444-444444444444"),
                columns: new[] { "CategoryId", "Description", "Name", "Price", "StockQuantity" },
                values: new object[] { new Guid("aaaaaaaa-2222-2222-2222-222222222222"), "Antybiotyk", "Amotaks", 29.99m, 40 });

            migrationBuilder.InsertData(
                table: "Medicines",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Price", "StockQuantity" },
                values: new object[] { new Guid("bbbbbbbb-5555-5555-5555-555555555555"), new Guid("aaaaaaaa-3333-3333-3333-333333333333"), "Syrop łagodzący kaszel", "Syrop na kaszel", 21.30m, 65 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "FirstName",
                value: "Wiesław");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-5555-5555-5555-555555555555"));

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "ApartmentNumber", "City", "HouseNumber", "PostalCode", "Street" },
                values: new object[,]
                {
                    { new Guid("55555555-2222-2222-2222-222222222222"), "2", "Białystok", "88", "15-077", "Warszawska" },
                    { new Guid("55555555-3333-3333-3333-333333333333"), "10", "Zambrów", "3", "18-300", "Sienkiewicza" }
                });

            migrationBuilder.UpdateData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-3333-3333-3333-333333333333"),
                columns: new[] { "CategoryId", "Description", "Name", "Price", "StockQuantity" },
                values: new object[] { new Guid("aaaaaaaa-2222-2222-2222-222222222222"), "Antybiotyk", "Amotaks", 29.99m, 40 });

            migrationBuilder.UpdateData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-4444-4444-4444-444444444444"),
                columns: new[] { "CategoryId", "Description", "Name", "Price", "StockQuantity" },
                values: new object[] { new Guid("aaaaaaaa-3333-3333-3333-333333333333"), "Syrop łagodzący kaszel", "Syrop na kaszel", 21.30m, 65 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "FirstName",
                value: "Piotr");
        }
    }
}
