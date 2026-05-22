using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MedicalCenter.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<MedicalLeave> MedicalLeaves{ get; set; }
        public DbSet<Referral> Referral { get; set; }

        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineCategory> MedicineCategories { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderRating> OrderRatings { get; set; }

        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DoctorDepartment> DoctorDepartments { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<DeliveryStatus> DeliveryStatuses { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<DoctorDepartment>()
                .HasIndex(dd => new { dd.DoctorId, dd.DepartmentId })
                .IsUnique();

            modelBuilder.Entity<MedicalRecord>()
                .HasIndex(mr => new { mr.PatientId, mr.DoctorId })
                .IsUnique();

            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.PatientId)
                .IsUnique();

            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.MedicineId })
                .IsUnique();

            modelBuilder.Entity<PrescriptionItem>()
                .HasIndex(pi => new { pi.PrescriptionId, pi.MedicineId })
                .IsUnique();

            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => new { oi.OrderId, oi.MedicineId })
                .IsUnique();

            modelBuilder.Entity<Delivery>()
                .HasIndex(d => d.OrderId)
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.PatientId, r.DoctorId })
                .IsUnique();

            modelBuilder.Entity<OrderRating>()
                .HasIndex(r => new { r.PatientId, r.OrderId })
                .IsUnique();

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Doctor" },
                new Role { Id = 3, Name = "Patient" },
                new Role { Id = 4, Name = "Courier" }
            );

            modelBuilder.Entity<AppointmentStatus>().HasData(
                new AppointmentStatus { Id = 1, Name = "Zaplanowana" },
                new AppointmentStatus { Id = 2, Name = "Zakończona" },
                new AppointmentStatus { Id = 3, Name = "Anulowana" }
            );

            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = 1, Name = "Nowe" },
                new OrderStatus { Id = 2, Name = "W realizacji" },
                new OrderStatus { Id = 3, Name = "Wysłane" },
                new OrderStatus { Id = 4, Name = "Zakończone" }
            );

            modelBuilder.Entity<DeliveryStatus>().HasData(
                new DeliveryStatus { Id = 1, Name = "Oczekuje na kuriera" },
                new DeliveryStatus { Id = 2, Name = "W drodze" },
                new DeliveryStatus { Id = 3, Name = "Dostarczono" }
            );




            // SPECIALIZATIONS
            var specjalizacjaKardiolog = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var specjalizacjaNeurolog = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var specjalizacjaPediatra = Guid.Parse("33333333-3333-3333-3333-333333333333");

            modelBuilder.Entity<Specialization>().HasData(
                new Specialization
                {
                    Id = specjalizacjaKardiolog,
                    Name = "Kardiolog"
                },
                new Specialization
                {
                    Id = specjalizacjaNeurolog,
                    Name = "Neurolog"
                },
                new Specialization
                {
                    Id = specjalizacjaPediatra,
                    Name = "Pediatra"
                }
            );

            // DEPARTMENTS
            var oddzialKardiologia = Guid.Parse("44444444-1111-1111-1111-111111111111");
            var oddzialNeurologia = Guid.Parse("44444444-2222-2222-2222-222222222222");
            var oddzialPediatria = Guid.Parse("44444444-3333-3333-3333-333333333333");

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = oddzialKardiologia,
                    Name = "Oddział Kardiologii"
                },
                new Department
                {
                    Id = oddzialNeurologia,
                    Name = "Oddział Neurologii"
                },
                new Department
                {
                    Id = oddzialPediatria,
                    Name = "Oddział Pediatrii"
                }
            );

            // ADDRESSES
            var adresPacjenta = Guid.Parse("55555555-1111-1111-1111-111111111111");
           

            modelBuilder.Entity<Address>().HasData(
                new Address
                {
                    Id = adresPacjenta,
                    Street = "Lipowa",
                    HouseNumber = "12",
                    ApartmentNumber = "5",
                    PostalCode = "15-424",
                    City = "Białystok"
                }
            );

            // USERS
            var adminUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var doctorUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var patientUserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            var courierUserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminUserId,
                    Email = "admin@medical.pl",
                    PasswordHash = "$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e",
                    FirstName = "Adam",
                    LastName = "Nowak",
                    Phone = "111222333",
                    RoleId = 1,
                    ProfilePicturePath = null
                },
                new User
                {
                    Id = doctorUserId,
                    Email = "lekarz@medical.pl",
                    PasswordHash = "$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e",
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Phone = "222333444",
                    RoleId = 2,
                    ProfilePicturePath = null
                },
                new User
                {
                    Id = patientUserId,
                    Email = "pacjent@medical.pl",
                    PasswordHash = "$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e",
                    FirstName = "Anna",
                    LastName = "Wiśniewska",
                    Phone = "333444555",
                    RoleId = 3,
                    ProfilePicturePath = null
                },
                new User
                {
                    Id = courierUserId,
                    Email = "kurier@medical.pl",
                    PasswordHash = "$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e",
                    FirstName = "Wiesław",
                    LastName = "Szybki",
                    Phone = "999888777",
                    RoleId = 4,
                    ProfilePicturePath = null
                }
            );

            // DOCTOR
            var doctorId = Guid.Parse("66666666-1111-1111-1111-111111111111");

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = doctorId,
                    UserId = doctorUserId,
                    LicenseNumber = "LEK123456",
                    SpecializationId = specjalizacjaKardiolog
                }
            );

            // DOCTOR DEPARTMENTS
            modelBuilder.Entity<DoctorDepartment>().HasData(
                new DoctorDepartment
                {
                    Id = Guid.Parse("77777777-1111-1111-1111-111111111111"),
                    DoctorId = doctorId,
                    DepartmentId = oddzialKardiologia
                },
                new DoctorDepartment
                {
                    Id = Guid.Parse("77777777-2222-2222-2222-222222222222"),
                    DoctorId = doctorId,
                    DepartmentId = oddzialNeurologia
                }
            );

            // PATIENT
            var patientId = Guid.Parse("88888888-1111-1111-1111-111111111111");

            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = patientId,
                    UserId = patientUserId,
                    Pesel = "99010112345",
                    BirthDate = new DateTime(1999, 1, 1),
                    AddressId = adresPacjenta
                }
            );

            // COURIER
            modelBuilder.Entity<Courier>().HasData(
                new Courier
                {
                    Id = Guid.Parse("99999999-1111-1111-1111-111111111111"),
                    UserId = courierUserId,
                    VehicleRegistration = "BI1234K"
                }
            );

            // MEDICINE CATEGORIES
            var przeciwboloweId = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111");
            var antybiotykiId = Guid.Parse("aaaaaaaa-2222-2222-2222-222222222222");
            var syropyId = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333");

            modelBuilder.Entity<MedicineCategory>().HasData(
                new MedicineCategory
                {
                    Id = przeciwboloweId,
                    Name = "Leki przeciwbólowe"
                },
                new MedicineCategory
                {
                    Id = antybiotykiId,
                    Name = "Antybiotyki"
                },
                new MedicineCategory
                {
                    Id = syropyId,
                    Name = "Syropy"
                }
            );

            // MEDICINES
            modelBuilder.Entity<Medicine>().HasData(
                new Medicine
                {
                    Id = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111"),
                    Name = "Apap Extra",
                    Price = 15.50m,
                    StockQuantity = 120,
                    Description = "Lek przeciwbólowy i przeciwgorączkowy",
                    CategoryId = przeciwboloweId
                },
                new Medicine
                {
                    Id = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222"),
                    Name = "Ibuprom Max",
                    Price = 12.99m,
                    StockQuantity = 85,
                    Description = "Silny lek przeciwbólowy",
                    CategoryId = przeciwboloweId
                },
                new Medicine
                {
                    Id = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"),
                    Name = "Maść na Ból Dupy",
                    Price = 999999.99m,
                    StockQuantity = 1,
                    Description = "Silny lek przeciwbólowy",
                    CategoryId = przeciwboloweId
                },
                new Medicine
                {
                    Id = Guid.Parse("bbbbbbbb-4444-4444-4444-444444444444"),
                    Name = "Amotaks",
                    Price = 29.99m,
                    StockQuantity = 40,
                    Description = "Antybiotyk",
                    CategoryId = antybiotykiId
                },
                new Medicine
                {
                    Id = Guid.Parse("bbbbbbbb-5555-5555-5555-555555555555"),
                    Name = "Syrop na kaszel",
                    Price = 21.30m,
                    StockQuantity = 65,
                    Description = "Syrop łagodzący kaszel",
                    CategoryId = syropyId
                }
            );
        }
    }
}
