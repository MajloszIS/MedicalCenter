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
                new User { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Email = "admin@medical.pl", PasswordHash = "admin123", FirstName = "Adam", LastName= "Nowak", Phone= "111222333", RoleId = 1},
                new User { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Email = "doktor@medical.pl", PasswordHash = "doktor123", FirstName = "Jan", LastName = "Kowalski", Phone = "222333444", RoleId = 2 },
                new User { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Email = "pacjent@medical.pl", PasswordHash = "pacjent123", FirstName = "Anna", LastName = "Wiśniewska", Phone = "333444555", RoleId = 3 }
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
        }
    }
}
