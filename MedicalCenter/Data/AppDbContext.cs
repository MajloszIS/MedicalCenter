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

        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineCategory> MedicineCategories { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Doctor" },
                new Role { Id = 3, Name = "Patient" },
                new Role { Id = 4, Name = "Courier" }
            );

            modelBuilder.Entity<Specialization>().HasData(
                new Specialization { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Kardiolog" },
                new Specialization { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Neurolog" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Email = "admin@medical.pl", PasswordHash = "$2a$11$wHXCchTbS3pO/OujL1VHQebwwG.cPIncjS2w7JHidEZqzLT05tg7e", FirstName = "Adam", LastName= "Nowak", Phone= "111222333", RoleId = 1},
                new User { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Email = "doktor@medical.pl", PasswordHash = "$2a$11$VmozT24fOt40zBIdkcNYFeO6z0sVfe2GdFOzyoSKVgSATzjNZSia6", FirstName = "Jan", LastName = "Kowalski", Phone = "222333444", RoleId = 2 },
                new User { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Email = "pacjent@medical.pl", PasswordHash = "$2a$11$SjBITGayq8gTCE4JLjt4becH4zr32rn5cixIlaqdJtSCvYwd1O/QC", FirstName = "Anna", LastName = "Wiśniewska", Phone = "333444555", RoleId = 3 }
                );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), LicenseNumber = "LEK123456", SpecializationId = Guid.Parse("11111111-1111-1111-1111-111111111111") }
            );

            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Pesel = "99010112345", BirthDate = new DateTime(1999, 1, 1) }
            );

            modelBuilder.Entity<AppointmentStatus>().HasData(
                new AppointmentStatus { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Name = "Zaplanowana" },
                new AppointmentStatus { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000000"), Name = "Zakończona" },
                new AppointmentStatus { Id = Guid.Parse("12345678-1234-1234-1234-123456789012"), Name = "Anulowana" }
            );

            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111"), Name = "Nowe" },
                new OrderStatus { Id = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222"), Name = "W realizacji" },
                new OrderStatus { Id = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"), Name = "Wysłane" },
                new OrderStatus { Id = Guid.Parse("bbbbbbbb-4444-4444-4444-444444444444"), Name = "Zakończone" }
            );

            modelBuilder.Entity<MedicineCategory>().HasData(
                new MedicineCategory { Id = Guid.Parse("cccccccc-1111-1111-1111-111111111111"), Name = "Leki przeciwbólowe" },
                new MedicineCategory { Id = Guid.Parse("cccccccc-2222-2222-2222-222222222222"), Name = "Syrop" },
                new MedicineCategory { Id = Guid.Parse("cccccccc-3333-3333-3333-333333333333"), Name = "Leki" }
            );

            modelBuilder.Entity<Medicine>().HasData(
                new Medicine { Id = Guid.Parse("dddddddd-1111-1111-1111-111111111111"), Name = "Apap Extra", Price = 15.50m, CategoryId = Guid.Parse("cccccccc-1111-1111-1111-111111111111") },
                new Medicine { Id = Guid.Parse("dddddddd-2222-2222-2222-222222222222"), Name = "Ibuprom Max", Price = 12.99m, CategoryId = Guid.Parse("cccccccc-1111-1111-1111-111111111111") },
                new Medicine { Id = Guid.Parse("dddddddd-3333-3333-3333-333333333333"), Name = "Rutinoscorbin", Price = 9.00m, CategoryId = Guid.Parse("cccccccc-3333-3333-3333-333333333333") },
                new Medicine { Id = Guid.Parse("dddddddd-4444-4444-4444-444444444444"), Name = "Syrop na kaszel", Price = 21.30m, CategoryId = Guid.Parse("cccccccc-2222-2222-2222-222222222222") }
            );

            modelBuilder.Entity<DeliveryStatus>().HasData(
            new DeliveryStatus { Id = Guid.Parse("eeeeeeee-1111-1111-1111-111111111111"), Name = "Oczekuje na kuriera" },
            new DeliveryStatus { Id = Guid.Parse("eeeeeeee-2222-2222-2222-222222222222"), Name = "W drodze" },
            new DeliveryStatus { Id = Guid.Parse("eeeeeeee-3333-3333-3333-333333333333"), Name = "Dostarczono" }
            );
        }
    }
}
