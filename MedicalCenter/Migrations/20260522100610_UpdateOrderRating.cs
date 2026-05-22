using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRatings_Doctors_DoctorId",
                table: "OrderRatings");

            migrationBuilder.DropIndex(
                name: "IX_OrderRatings_DoctorId",
                table: "OrderRatings");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "OrderRatings");

            migrationBuilder.CreateIndex(
                name: "IX_OrderRatings_OrderId",
                table: "OrderRatings",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRatings_Orders_OrderId",
                table: "OrderRatings",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRatings_Orders_OrderId",
                table: "OrderRatings");

            migrationBuilder.DropIndex(
                name: "IX_OrderRatings_OrderId",
                table: "OrderRatings");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "OrderRatings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrderRatings_DoctorId",
                table: "OrderRatings",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRatings_Doctors_DoctorId",
                table: "OrderRatings",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
