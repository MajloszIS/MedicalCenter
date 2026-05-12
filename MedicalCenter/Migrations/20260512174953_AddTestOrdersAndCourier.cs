using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddTestOrdersAndCourier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CourierId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "PatientId", "StatusId", "TotalPrice" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("bbbbbbbb-1111-1111-1111-111111111111"), 31.00m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("bbbbbbbb-2222-2222-2222-222222222222"), 12.99m },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("bbbbbbbb-4444-4444-4444-444444444444"), 9.00m }
                });

            migrationBuilder.InsertData(
                table: "Deliveries",
                columns: new[] { "Id", "CourierId", "OrderId", "StatusId" },
                values: new object[,]
                {
                    { new Guid("55555555-1111-1111-1111-111111111111"), null, new Guid("22222222-2222-2222-2222-222222222222"), new Guid("eeeeeeee-1111-1111-1111-111111111111") },
                    { new Guid("55555555-2222-2222-2222-222222222222"), new Guid("ffffffff-2222-2222-2222-222222222222"), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("eeeeeeee-3333-3333-3333-333333333333") }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "MedicineId", "OrderId", "Quantity" },
                values: new object[,]
                {
                    { new Guid("66666666-1111-1111-1111-111111111111"), new Guid("dddddddd-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), 2 },
                    { new Guid("66666666-2222-2222-2222-222222222222"), new Guid("dddddddd-2222-2222-2222-222222222222"), new Guid("22222222-2222-2222-2222-222222222222"), 1 },
                    { new Guid("66666666-3333-3333-3333-333333333333"), new Guid("dddddddd-3333-3333-3333-333333333333"), new Guid("33333333-3333-3333-3333-333333333333"), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Deliveries",
                keyColumn: "Id",
                keyValue: new Guid("55555555-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Deliveries",
                keyColumn: "Id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: new Guid("66666666-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: new Guid("66666666-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: new Guid("66666666-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CourierId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
