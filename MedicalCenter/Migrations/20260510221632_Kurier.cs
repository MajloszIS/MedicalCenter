using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class Kurier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_DeliveryStatusId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_OrderStatuses_StatusId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_DeliveryStatusId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryStatusId",
                table: "Deliveries");

            migrationBuilder.InsertData(
                table: "DeliveryStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("eeeeeeee-1111-1111-1111-111111111111"), "Oczekuje na kuriera" },
                    { new Guid("eeeeeeee-2222-2222-2222-222222222222"), "W drodze" },
                    { new Guid("eeeeeeee-3333-3333-3333-333333333333"), "Dostarczono" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_StatusId",
                table: "Deliveries",
                column: "StatusId",
                principalTable: "DeliveryStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_StatusId",
                table: "Deliveries");

            migrationBuilder.DeleteData(
                table: "DeliveryStatuses",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "DeliveryStatuses",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "DeliveryStatuses",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-3333-3333-3333-333333333333"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryStatusId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryStatusId",
                table: "Deliveries",
                column: "DeliveryStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_DeliveryStatusId",
                table: "Deliveries",
                column: "DeliveryStatusId",
                principalTable: "DeliveryStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_OrderStatuses_StatusId",
                table: "Deliveries",
                column: "StatusId",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
