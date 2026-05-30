using Bogus;
using MedicalCenter.Data;
using MedicalCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Services
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;
        public DataSeeder(AppDbContext context) => _context = context;

        public async Task SeedAsync(
            int patientCount = 5000,
            int doctorCount = 30,
            int courierCount = 10,
            int medicineCount = 500)
        {
            Randomizer.Seed = new Random(12345);  // deterministyczne wyniki

            // Sprawdź czy słowniki istnieją - mają być z seed data aplikacji
            var patientRole = await _context.Roles.FirstAsync(r => r.Name == "Patient");
            var doctorRole = await _context.Roles.FirstAsync(r => r.Name == "Doctor");
            var courierRole = await _context.Roles.FirstAsync(r => r.Name == "Courier");

            Console.WriteLine("Seedowanie słowników...");
            await SeedSpecializations();
            await SeedDepartments();
            await SeedMedicineCategories();

            Console.WriteLine("Seedowanie adresów...");
            var addresses = await SeedAddresses(patientCount + doctorCount + courierCount);

            Console.WriteLine($"Seedowanie {patientCount} pacjentów...");
            await SeedPatients(patientCount, patientRole.Id, addresses);

            Console.WriteLine($"Seedowanie {doctorCount} lekarzy...");
            await SeedDoctors(doctorCount, doctorRole.Id);

            Console.WriteLine($"Seedowanie {courierCount} kurierów...");
            await SeedCouriers(courierCount, courierRole.Id);

            Console.WriteLine($"Seedowanie {medicineCount} leków...");
            await SeedMedicines(medicineCount);

            Console.WriteLine("Seedowanie powiązań lekarz-departament...");
            await SeedDoctorDepartments();

            Console.WriteLine("Done.");
        }

        // ---------- SŁOWNIKI ----------

        private async Task SeedSpecializations()
        {
            if (await _context.Specializations.AnyAsync()) return;

            var names = new[]
            {
                "Kardiolog", "Neurolog", "Pediatra", "Dermatolog", "Ortopeda",
                "Okulista", "Laryngolog", "Ginekolog", "Endokrynolog",
                "Psychiatra", "Urolog", "Onkolog", "Reumatolog",
                "Gastrolog", "Chirurg"
            };

            _context.Specializations.AddRange(
                names.Select(n => new Specialization { Name = n }));
            await _context.SaveChangesAsync();
        }

        private async Task SeedDepartments()
        {
            if (await _context.Departments.AnyAsync()) return;

            var names = new[]
            {
                "Oddział Wewnętrzny", "Oddział Chirurgiczny", "Oddział Pediatryczny",
                "Oddział Kardiologiczny", "Oddział Neurologiczny", "Poradnia Specjalistyczna",
                "Izba Przyjęć", "Diagnostyka Obrazowa"
            };

            _context.Departments.AddRange(
                names.Select(n => new Department { Name = n }));
            await _context.SaveChangesAsync();
        }

        private async Task SeedMedicineCategories()
        {
            if (await _context.MedicineCategories.AnyAsync()) return;

            var names = new[]
            {
                "Przeciwbólowe", "Antybiotyki", "Przeciwzapalne", "Witaminy i suplementy",
                "Krążenie", "Układ pokarmowy", "Układ nerwowy", "Dermatologia",
                "Alergia", "Diabetologia", "Probiotyki", "Hormony"
            };

            _context.MedicineCategories.AddRange(
                names.Select(n => new MedicineCategory { Name = n }));
            await _context.SaveChangesAsync();
        }

        // ---------- ADRESY ----------

        private async Task<List<Address>> SeedAddresses(int count)
        {
            var cities = new[]
            {
                "Białystok", "Warszawa", "Kraków", "Gdańsk", "Poznań",
                "Łódź", "Wrocław", "Lublin", "Katowice", "Szczecin", "Bydgoszcz"
            };

            var faker = new Faker<Address>("pl")
                .RuleFor(a => a.Id, _ => Guid.NewGuid())
                .RuleFor(a => a.Street, f => f.Address.StreetName())
                .RuleFor(a => a.HouseNumber, f => f.Random.Number(1, 200).ToString())
                .RuleFor(a => a.ApartmentNumber,
                    f => f.Random.Bool(0.4f) ? f.Random.Number(1, 50).ToString() : null)
                .RuleFor(a => a.PostalCode,
                    f => $"{f.Random.Number(10, 99)}-{f.Random.Number(100, 999)}")
                .RuleFor(a => a.City, f => f.PickRandom(cities));

            var addresses = faker.Generate(count);
            await _context.Addresses.AddRangeAsync(addresses);
            await _context.SaveChangesAsync();
            return addresses;
        }

        // ---------- PACJENCI ----------

        private async Task SeedPatients(int count, int roleId, List<Address> addresses)
        {
            var users = new List<User>(count);
            var patients = new List<Patient>(count);
            var dummyHash = BCrypt.Net.BCrypt.HashPassword("Seeded123!");

            var userFaker = new Faker<User>("pl")
                .RuleFor(u => u.Id, _ => Guid.NewGuid())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Phone, f => f.Random.Replace("#########"))
                .RuleFor(u => u.PasswordHash, _ => dummyHash)
                .RuleFor(u => u.RoleId, _ => roleId);

            for (int i = 0; i < count; i++)
            {
                var u = userFaker.Generate();
                // unikalny email per pacjent
                u.Email = $"patient{i:D5}@seed.local";
                users.Add(u);

                patients.Add(new Patient
                {
                    Id = Guid.NewGuid(),
                    UserId = u.Id,
                    AddressId = addresses[i % addresses.Count].Id,
                    BirthDate = new Faker().Date.Between(
                        new DateTime(1940, 1, 1),
                        new DateTime(2008, 1, 1)),
                    Pesel = GeneratePesel(new Faker())
                });
            }

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();
        }

        // ---------- LEKARZE ----------

        private async Task SeedDoctors(int count, int roleId)
        {
            var specializationIds = await _context.Specializations
                .Select(s => s.Id).ToListAsync();

            var users = new List<User>(count);
            var doctors = new List<Doctor>(count);
            var dummyHash = BCrypt.Net.BCrypt.HashPassword("Seeded123!");
            var f = new Faker("pl");

            for (int i = 0; i < count; i++)
            {
                var u = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = f.Name.FirstName(),
                    LastName = f.Name.LastName(),
                    Email = $"doctor{i:D3}@seed.local",
                    Phone = f.Random.Replace("#########"),
                    PasswordHash = dummyHash,
                    RoleId = roleId
                };
                users.Add(u);

                doctors.Add(new Doctor
                {
                    Id = Guid.NewGuid(),
                    UserId = u.Id,
                    LicenseNumber = $"LEK{f.Random.Number(100000, 999999)}",
                    SpecializationId = f.PickRandom(specializationIds)
                });
            }

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();
        }

        // ---------- KURIERZY ----------

        private async Task SeedCouriers(int count, int roleId)
        {
            var users = new List<User>(count);
            var couriers = new List<Courier>(count);
            var dummyHash = BCrypt.Net.BCrypt.HashPassword("Seeded123!");
            var f = new Faker("pl");

            for (int i = 0; i < count; i++)
            {
                var u = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = f.Name.FirstName(),
                    LastName = f.Name.LastName(),
                    Email = $"courier{i:D3}@seed.local",
                    Phone = f.Random.Replace("#########"),
                    PasswordHash = dummyHash,
                    RoleId = roleId
                };
                users.Add(u);

                couriers.Add(new Courier
                {
                    Id = Guid.NewGuid(),
                    UserId = u.Id,
                    VehicleRegistration = $"{f.Random.String2(2, "ABCDEFGHIJKLMNOPRSTUWXYZ")}" +
                                          $"{f.Random.Number(10000, 99999)}"
                });
            }

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            await _context.Couriers.AddRangeAsync(couriers);
            await _context.SaveChangesAsync();
        }

        // ---------- LEKI ----------

        private async Task SeedMedicines(int count)
        {
            var categoryIds = await _context.MedicineCategories
                .Select(c => c.Id).ToListAsync();

            // Stałe prefiksy dla bardziej "leczniczych" nazw
            var prefixes = new[]
            {
                "Apap", "Ibuprom", "Aspirin", "Polopiryna", "Paracetamol",
                "Amoxicillin", "Augmentin", "Cetiryzyna", "Loratadyna",
                "Metformin", "Ramipril", "Bisoprolol", "Omeprazol", "Pantoprazol",
                "Witamina D", "Witamina C", "Magnez", "Cynk", "Probiotyk"
            };
            var suffixes = new[] { "Forte", "Plus", "Max", "Active", "DUO", "Standard", "" };

            var medicineFaker = new Faker<Medicine>("pl")
                .RuleFor(m => m.Id, _ => Guid.NewGuid())
                .RuleFor(m => m.Name, f =>
                    $"{f.PickRandom(prefixes)} {f.PickRandom(suffixes)} {f.Random.Number(50, 500)}mg".Trim())
                .RuleFor(m => m.Price, f => Math.Round(f.Random.Decimal(5m, 200m), 2))
                .RuleFor(m => m.StockQuantity, f => f.Random.Number(0, 1000))
                .RuleFor(m => m.Description, f => f.Lorem.Sentence(8))
                .RuleFor(m => m.CategoryId, f => f.PickRandom(categoryIds));

            var medicines = medicineFaker.Generate(count);
            await _context.Medicines.AddRangeAsync(medicines);
            await _context.SaveChangesAsync();
        }

        // ---------- LEKARZ-DEPARTAMENT (M:N) ----------

        private async Task SeedDoctorDepartments()
        {
            var doctorIds = await _context.Doctors.Select(d => d.Id).ToListAsync();
            var departmentIds = await _context.Departments.Select(d => d.Id).ToListAsync();
            var f = new Faker();

            var assignments = new List<DoctorDepartment>();
            foreach (var doctorId in doctorIds)
            {
                // każdy lekarz w 1-2 departamentach
                var count = f.Random.Number(1, 2);
                var chosenDepts = f.PickRandom(departmentIds, count).Distinct();

                foreach (var deptId in chosenDepts)
                {
                    assignments.Add(new DoctorDepartment
                    {
                        Id = Guid.NewGuid(),
                        DoctorId = doctorId,
                        DepartmentId = deptId
                    });
                }
            }

            await _context.DoctorDepartments.AddRangeAsync(assignments);
            await _context.SaveChangesAsync();
        }

        // ---------- HELPER ----------

        private string GeneratePesel(Faker f)
        {
            // 11 cyfr losowych - bez poprawnej sumy kontrolnej
            // dla seeda projektowego wystarczy
            return string.Concat(Enumerable.Range(0, 11)
                .Select(_ => f.Random.Number(0, 9).ToString()));
        }
    }
}