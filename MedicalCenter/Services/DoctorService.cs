using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISpecializationsRepository _specializationsRepository;
        private readonly IDepartmentRepository _departmentRepository;
        public DoctorService(IDoctorRepository doctorRepository, IUserRepository userRepository, ISpecializationsRepository specializationsRepository, IDepartmentRepository departmentRepository)
        {
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _specializationsRepository = specializationsRepository;
            _departmentRepository = departmentRepository;
        }
        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllDoctorsAsync();

            var doctorDtos = doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FirstName = d.User.FirstName,
                LastName = d.User.LastName,
                Phone = d.User.Phone,
                SpecializationName = d.Specialization.Name
            }).ToList();

            return doctorDtos;
        }
        public async Task<DoctorDto> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(id) ?? throw new Exception("Nie znaleziono lekarza.");

            var doctorDto = new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.User.FirstName,
                LastName = doctor.User.LastName,
                Phone = doctor.User.Phone,
                SpecializationName = doctor.Specialization.Name,
            }; 
            return doctorDto;
        }

        public async Task<DoctorDto> GetDoctorByUserIdAsync(Guid userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId) ?? throw new Exception("Nie znaleziono lekarza.");
            var doctorDto = new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.User.FirstName,
                LastName = doctor.User.LastName,
                Phone = doctor.User.Phone,
                SpecializationName = doctor.Specialization.Name
            };
            return doctorDto;
        }
        public async Task CreateDoctorAsync(AdminCreateDoctorDto dto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                PasswordHash = passwordHash,
                RoleId = 2,
            };

            await _userRepository.CreateUserAsync(user);

            // Pobierz specjalizację na podstawie nazwy
            var specialization = await _specializationsRepository.GetSpecializationByNameAsync(dto.SpecializationName);
            if (specialization == null)
            {
                throw new Exception("Nie znaleziono takiej specjalizacji.");
            }

            if (dto.SelectedDepartmentIds == null || !dto.SelectedDepartmentIds.Any())
            {
                throw new Exception("Nie podano żadnego departamnetu.");
            }

            var selectedDepartments = await _departmentRepository.GetDepartmentsByIdsAsync(dto.SelectedDepartmentIds);

            if (selectedDepartments.Count != dto.SelectedDepartmentIds.Count)
            {
                throw new Exception("Jeden lub więcej departamentów nie istnieje.");
            }

            var doctor = new Doctor
            {
                LicenseNumber = dto.LicenseNumber,
                SpecializationId = specialization.Id,
                UserId = user.Id,
            };

            doctor.UserId = user.Id;
            doctor.DoctorDepartments = selectedDepartments.Select(d => new DoctorDepartment
            {
                DoctorId = doctor.Id,
                DepartmentId = d.Id
            }).ToList();

            await _doctorRepository.CreateDoctorAsync(doctor);
        }
        public async Task DeleteDoctorAsync(Guid DoctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(DoctorId) ?? throw new Exception("Nie znaleziono lekarza.");
            var user = await _userRepository.GetUserByIdAsync(doctor.UserId) ?? throw new Exception("Nie znaleziono użytkownika.");
            
            await _doctorRepository.DeleteDoctorAsync(doctor.Id);
            await _userRepository.DeleteUserAsync(user.Id);
            
        }
        public async Task<DoctorProfileDto> GetDoctorProfileAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception("Nie znaleziono użytkownika.");
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(id) ?? throw new Exception("Nie znaleziono lekarza.");

            var doctorProfileDto = new DoctorProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                ProfilePicturePath = user.ProfilePicturePath,
                LicenseNumber = doctor.LicenseNumber,
                SpecializationName = doctor.Specialization.Name,
                SelectedDepartment = doctor.DoctorDepartments.Select(dd => new DepartmentDto
                {
                    Id = dd.Department.Id,
                    Name = dd.Department.Name
                }).ToList()
            };
            return doctorProfileDto;
        }
        public async Task UpdateDoctorProfileAsync(Guid id, UpdateDoctorProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception("Nie znaleziono użytkownika.");
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(id) ?? throw new Exception("Nie znaleziono doktora.");

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Email = dto.Email ?? user.Email;
            doctor.LicenseNumber = dto.LicenseNumber ?? doctor.LicenseNumber;

            if (dto.SpecializationName != null)
            {
                var specialization = await _specializationsRepository.GetSpecializationByNameAsync(dto.SpecializationName) ?? throw new Exception("Nie znaleziono specjalizacji.");
                doctor.SpecializationId = specialization.Id;
            }

            if (dto.SelectedDepartmentIds != null)
            {
                await _doctorRepository.UpdateDoctorDepartmentsAsync(doctor.Id, dto.SelectedDepartmentIds);
            }

            await _userRepository.UpdateUserAsync(user);
            await _doctorRepository.UpdateDoctorAsync(doctor);
        }
        public async Task<List<SpecializationDto>> GetAllSpecializationsAsync()
        {
            var specializations = await _specializationsRepository.GetAllSpecializationsAsync();
            var specializationDtos = specializations.Select(s => new SpecializationDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
            return specializationDtos;
        }
        public async Task<List<DoctorDto>> GetDoctorsBySpecializationAsync(string specializationName)
        {
            var doctors = await _doctorRepository.GetDoctorsBySpecializationAsync(specializationName);
            var doctorDtos = doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FirstName = d.User.FirstName,
                LastName = d.User.LastName,
                Phone = d.User.Phone,
                SpecializationName = d.Specialization.Name
            }).ToList();
            return doctorDtos;
        }
        public async Task<DoctorWorkloadDto?> GetDoctorWorkloadAsync(Guid doctorId, DateTime dateFrom, DateTime dateTo)
        {
            return await _doctorRepository.GetDoctorWorkloadAsync(doctorId, dateFrom, dateTo);
        }
    }
}
