using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatusIdsToIntAndUpdateShipmentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Wyczyść dane (na czystej bazie i tak nic nie ma, ale dla pewności)
            migrationBuilder.Sql("DELETE FROM Appointments");
            migrationBuilder.Sql("DELETE FROM OrderItems");
            migrationBuilder.Sql("DELETE FROM Deliveries");
            migrationBuilder.Sql("DELETE FROM Orders");
            migrationBuilder.Sql("DELETE FROM AppointmentStatuses");
            migrationBuilder.Sql("DELETE FROM DeliveryStatuses");
            migrationBuilder.Sql("DELETE FROM OrderStatuses");

            // === Drop unique indexes z innych tabel (cofamy do non-unique na końcu) ===
            migrationBuilder.DropIndex(name: "IX_Reviews_PatientId", table: "Reviews");
            migrationBuilder.DropIndex(name: "IX_PrescriptionItems_PrescriptionId", table: "PrescriptionItems");
            migrationBuilder.DropIndex(name: "IX_OrderItems_OrderId", table: "OrderItems");
            migrationBuilder.DropIndex(name: "IX_Deliveries_OrderId", table: "Deliveries");
            migrationBuilder.DropIndex(name: "IX_Carts_PatientId", table: "Carts");
            migrationBuilder.DropIndex(name: "IX_CartItems_CartId", table: "CartItems");

            // === Drop FK ze tabel wskazujących na statusy ===
            migrationBuilder.DropForeignKey(name: "FK_Orders_OrderStatuses_StatusId", table: "Orders");
            migrationBuilder.DropForeignKey(name: "FK_Deliveries_DeliveryStatuses_StatusId", table: "Deliveries");
            migrationBuilder.DropForeignKey(name: "FK_Appointments_AppointmentStatuses_StatusId", table: "Appointments");

            // === Drop indeksów na StatusId (tworzonych automatycznie przez EF dla FK) ===
            migrationBuilder.DropIndex(name: "IX_Orders_StatusId", table: "Orders");
            migrationBuilder.DropIndex(name: "IX_Deliveries_StatusId", table: "Deliveries");
            migrationBuilder.DropIndex(name: "IX_Appointments_StatusId", table: "Appointments");

            // === Drop kolumny StatusId w tabelach wskazujących ===
            migrationBuilder.DropColumn(name: "StatusId", table: "Orders");
            migrationBuilder.DropColumn(name: "StatusId", table: "Deliveries");
            migrationBuilder.DropColumn(name: "StatusId", table: "Appointments");

            // === Drop PK i kolumny Id w tabelach statusów ===
            migrationBuilder.DropPrimaryKey(name: "PK_OrderStatuses", table: "OrderStatuses");
            migrationBuilder.DropPrimaryKey(name: "PK_DeliveryStatuses", table: "DeliveryStatuses");
            migrationBuilder.DropPrimaryKey(name: "PK_AppointmentStatuses", table: "AppointmentStatuses");

            migrationBuilder.DropColumn(name: "Id", table: "OrderStatuses");
            migrationBuilder.DropColumn(name: "Id", table: "DeliveryStatuses");
            migrationBuilder.DropColumn(name: "Id", table: "AppointmentStatuses");

            // === Add nowa kolumna Id (int IDENTITY) w tabelach statusów ===
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrderStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DeliveryStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AppointmentStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // === Add PK na nowych kolumnach Id ===
            migrationBuilder.AddPrimaryKey(name: "PK_OrderStatuses", table: "OrderStatuses", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_DeliveryStatuses", table: "DeliveryStatuses", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_AppointmentStatuses", table: "AppointmentStatuses", column: "Id");

            // === Add nowa kolumna StatusId (int) w tabelach wskazujących ===
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // === Pozostałe zmiany schema (nie dotyczą statusów) ===
            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "OrderItems",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: true);

            // === Seedy statusów ===
            migrationBuilder.InsertData(
                table: "AppointmentStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
            { 1, "Zaplanowana" },
            { 2, "Zakończona" },
            { 3, "Anulowana" }
                });

            migrationBuilder.InsertData(
                table: "DeliveryStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
            { 1, "Oczekuje na kuriera" },
            { 2, "W drodze" },
            { 3, "Dostarczono" }
                });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
            { 1, "Nowe" },
            { 2, "W realizacji" },
            { 3, "Wysłane" },
            { 4, "Zakończone" }
                });

            
            // === Recreate indeksów na StatusId ===
            migrationBuilder.CreateIndex(name: "IX_Orders_StatusId", table: "Orders", column: "StatusId");
            migrationBuilder.CreateIndex(name: "IX_Deliveries_StatusId", table: "Deliveries", column: "StatusId");
            migrationBuilder.CreateIndex(name: "IX_Appointments_StatusId", table: "Appointments", column: "StatusId");

            // === Recreate FK ===
            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatuses_StatusId",
                table: "Orders",
                column: "StatusId",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryStatuses_StatusId",
                table: "Deliveries",
                column: "StatusId",
                principalTable: "DeliveryStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentStatuses_StatusId",
                table: "Appointments",
                column: "StatusId",
                principalTable: "AppointmentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // === Unique indexy (nowe) ===
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PatientId_DoctorId",
                table: "Reviews",
                columns: new[] { "PatientId", "DoctorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_PrescriptionId_MedicineId",
                table: "PrescriptionItems",
                columns: new[] { "PrescriptionId", "MedicineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_MedicineId",
                table: "OrderItems",
                columns: new[] { "OrderId", "MedicineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PatientId",
                table: "Carts",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_MedicineId",
                table: "CartItems",
                columns: new[] { "CartId", "MedicineId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_PatientId_DoctorId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionItems_PrescriptionId_MedicineId",
                table: "PrescriptionItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_MedicineId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PatientId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_MedicineId",
                table: "CartItems");

            migrationBuilder.DeleteData(
                table: "AppointmentStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AppointmentStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AppointmentStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DeliveryStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DeliveryStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DeliveryStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Deliveries");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "OrderStatuses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "StatusId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DeliveryStatuses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "StatusId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AppointmentStatuses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<Guid>(
                name: "StatusId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "AppointmentStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("12345678-1234-1234-1234-123456789012"), "Anulowana" },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000000"), "Zakończona" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Zaplanowana" }
                });

            migrationBuilder.UpdateData(
                table: "Deliveries",
                keyColumn: "Id",
                keyValue: new Guid("55555555-1111-1111-1111-111111111111"),
                column: "StatusId",
                value: new Guid("eeeeeeee-1111-1111-1111-111111111111"));

            migrationBuilder.UpdateData(
                table: "Deliveries",
                keyColumn: "Id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"),
                column: "StatusId",
                value: new Guid("eeeeeeee-3333-3333-3333-333333333333"));

            migrationBuilder.InsertData(
                table: "DeliveryStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("eeeeeeee-1111-1111-1111-111111111111"), "Oczekuje na kuriera" },
                    { new Guid("eeeeeeee-2222-2222-2222-222222222222"), "W drodze" },
                    { new Guid("eeeeeeee-3333-3333-3333-333333333333"), "Dostarczono" }
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

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "StatusId",
                value: new Guid("bbbbbbbb-1111-1111-1111-111111111111"));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "StatusId",
                value: new Guid("bbbbbbbb-2222-2222-2222-222222222222"));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "StatusId",
                value: new Guid("bbbbbbbb-4444-4444-4444-444444444444"));

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PatientId",
                table: "Reviews",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_PrescriptionId",
                table: "PrescriptionItems",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PatientId",
                table: "Carts",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");
        }
    }
}
