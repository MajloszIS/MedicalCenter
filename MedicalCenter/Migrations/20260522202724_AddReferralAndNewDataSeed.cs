using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralAndNewDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "Doctors",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-4444-4444-4444-444444444444"));

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
                table: "Couriers",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-2222-2222-2222-222222222222"));

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

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-1111-1111-1111-111111111111"));

            migrationBuilder.CreateTable(
                name: "Referral",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetSpecialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referral", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Referral_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referral_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "ApartmentNumber", "City", "HouseNumber", "PostalCode", "Street" },
                values: new object[,]
                {
                    { new Guid("55555555-1111-1111-1111-111111111111"), "5", "Białystok", "12", "15-424", "Lipowa" },
                    { new Guid("55555555-2222-2222-2222-222222222222"), "2", "Białystok", "88", "15-077", "Warszawska" },
                    { new Guid("55555555-3333-3333-3333-333333333333"), "10", "Zambrów", "3", "18-300", "Sienkiewicza" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("44444444-1111-1111-1111-111111111111"), "Oddział Kardiologii" },
                    { new Guid("44444444-2222-2222-2222-222222222222"), "Oddział Neurologii" },
                    { new Guid("44444444-3333-3333-3333-333333333333"), "Oddział Pediatrii" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "LicenseNumber", "SpecializationId", "UserId" },
                values: new object[] { new Guid("66666666-1111-1111-1111-111111111111"), "LEK123456", new Guid("11111111-1111-1111-1111-111111111111"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.InsertData(
                table: "MedicineCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-1111-1111-111111111111"), "Leki przeciwbólowe" },
                    { new Guid("aaaaaaaa-2222-2222-2222-222222222222"), "Antybiotyki" },
                    { new Guid("aaaaaaaa-3333-3333-3333-333333333333"), "Syropy" }
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "Pediatra" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "PasswordHash",
                value: "admin123");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "lekarz@medical.pl", "doctor123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "PasswordHash",
                value: "patient123");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "Phone", "ProfilePicturePath", "RoleId" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "kurier@medical.pl", "Piotr", "Szybki", "courier123", "999888777", null, 4 });

            migrationBuilder.InsertData(
                table: "Couriers",
                columns: new[] { "Id", "UserId", "VehicleRegistration" },
                values: new object[] { new Guid("99999999-1111-1111-1111-111111111111"), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "BI1234K" });

            migrationBuilder.InsertData(
                table: "DoctorDepartments",
                columns: new[] { "Id", "DepartmentId", "DoctorId" },
                values: new object[,]
                {
                    { new Guid("77777777-1111-1111-1111-111111111111"), new Guid("44444444-1111-1111-1111-111111111111"), new Guid("66666666-1111-1111-1111-111111111111") },
                    { new Guid("77777777-2222-2222-2222-222222222222"), new Guid("44444444-2222-2222-2222-222222222222"), new Guid("66666666-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Medicines",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-1111-1111-1111-111111111111"), "Lek przeciwbólowy i przeciwgorączkowy", "Apap Extra", 15.50m, 120 },
                    { new Guid("bbbbbbbb-2222-2222-2222-222222222222"), new Guid("aaaaaaaa-1111-1111-1111-111111111111"), "Silny lek przeciwbólowy", "Ibuprom Max", 12.99m, 85 },
                    { new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-2222-2222-2222-222222222222"), "Antybiotyk", "Amotaks", 29.99m, 40 },
                    { new Guid("bbbbbbbb-4444-4444-4444-444444444444"), new Guid("aaaaaaaa-3333-3333-3333-333333333333"), "Syrop łagodzący kaszel", "Syrop na kaszel", 21.30m, 65 }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "AddressId", "BirthDate", "Pesel", "UserId" },
                values: new object[] { new Guid("88888888-1111-1111-1111-111111111111"), new Guid("55555555-1111-1111-1111-111111111111"), new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "99010112345", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.CreateIndex(
                name: "IX_Referral_DoctorId",
                table: "Referral",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Referral_PatientId",
                table: "Referral",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Referral");

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("55555555-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Couriers",
                keyColumn: "Id",
                keyValue: new Guid("99999999-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("44444444-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "DoctorDepartments",
                keyColumn: "Id",
                keyValue: new Guid("77777777-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "DoctorDepartments",
                keyColumn: "Id",
                keyValue: new Guid("77777777-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Medicines",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("88888888-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("55555555-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("44444444-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("44444444-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: new Guid("66666666-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "MedicineCategories",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "ApartmentNumber", "City", "HouseNumber", "PostalCode", "Street" },
                values: new object[] { new Guid("aaaa1111-1111-1111-1111-111111111111"), null, null, null, null, null });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "LicenseNumber", "SpecializationId", "UserId" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "LEK123456", new Guid("11111111-1111-1111-1111-111111111111"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.InsertData(
                table: "MedicineCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("cccccccc-1111-1111-1111-111111111111"), "Leki przeciwbólowe" },
                    { new Guid("cccccccc-2222-2222-2222-222222222222"), "Syrop" },
                    { new Guid("cccccccc-3333-3333-3333-333333333333"), "Leki" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "PasswordHash",
                value: "$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "doktor@medical.pl", "$2a$11$VmozT24fOt40zBIdkcNYFeO6z0sVfe2GdFOzyoSKVgSATzjNZSia6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "PasswordHash",
                value: "$2a$11$SjBITGayq8gTCE4JLjt4becH4zr32rn5cixIlaqdJtSCvYwd1O/QC");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "Phone", "ProfilePicturePath", "RoleId" },
                values: new object[] { new Guid("ffffffff-1111-1111-1111-111111111111"), "kurier@medical.pl", "Szybki", "Wiesiek", "$2a$11$VmozT24fOt40zBIdkcNYFeO6z0sVfe2GdFOzyoSKVgSATzjNZSia6", "999888777", null, 4 });

            migrationBuilder.InsertData(
                table: "Couriers",
                columns: new[] { "Id", "UserId", "VehicleRegistration" },
                values: new object[] { new Guid("ffffffff-2222-2222-2222-222222222222"), new Guid("ffffffff-1111-1111-1111-111111111111"), null });

            migrationBuilder.InsertData(
                table: "Medicines",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { new Guid("dddddddd-1111-1111-1111-111111111111"), new Guid("cccccccc-1111-1111-1111-111111111111"), null, "Apap Extra", 15.50m, 0 },
                    { new Guid("dddddddd-2222-2222-2222-222222222222"), new Guid("cccccccc-1111-1111-1111-111111111111"), null, "Ibuprom Max", 12.99m, 0 },
                    { new Guid("dddddddd-3333-3333-3333-333333333333"), new Guid("cccccccc-3333-3333-3333-333333333333"), null, "Rutinoscorbin", 9.00m, 0 },
                    { new Guid("dddddddd-4444-4444-4444-444444444444"), new Guid("cccccccc-2222-2222-2222-222222222222"), null, "Syrop na kaszel", 21.30m, 0 }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "AddressId", "BirthDate", "Pesel", "UserId" },
                values: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("aaaa1111-1111-1111-1111-111111111111"), new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "99010112345", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "PatientId", "StatusId", "StripeSessionId", "TotalPrice" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 1, null, 31.00m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 2, null, 12.99m },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 4, null, 9.00m }
                });

            migrationBuilder.InsertData(
                table: "Deliveries",
                columns: new[] { "Id", "CourierId", "DeliveredAt", "OrderId", "StatusId" },
                values: new object[,]
                {
                    { new Guid("55555555-1111-1111-1111-111111111111"), null, null, new Guid("22222222-2222-2222-2222-222222222222"), 1 },
                    { new Guid("55555555-2222-2222-2222-222222222222"), new Guid("ffffffff-2222-2222-2222-222222222222"), null, new Guid("33333333-3333-3333-3333-333333333333"), 3 }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "MedicineId", "OrderId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("66666666-1111-1111-1111-111111111111"), new Guid("dddddddd-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), 2, 0m },
                    { new Guid("66666666-2222-2222-2222-222222222222"), new Guid("dddddddd-2222-2222-2222-222222222222"), new Guid("22222222-2222-2222-2222-222222222222"), 1, 0m },
                    { new Guid("66666666-3333-3333-3333-333333333333"), new Guid("dddddddd-3333-3333-3333-333333333333"), new Guid("33333333-3333-3333-3333-333333333333"), 1, 0m }
                });
        }
    }
}
