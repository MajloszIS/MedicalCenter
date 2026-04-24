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
        public DoctorService(IDoctorRepository doctorRepository, IUserRepository userRepository, ISpecializationsRepository specializationsRepository)
        {
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _specializationsRepository = specializationsRepository;
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
            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return null;
            }
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

        public async Task<DoctorDto> GetDoctorByUserIdAsync(Guid userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
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
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                PasswordHash = passwordHash,
                RoleId = 2
            };

            await _userRepository.CreateUserAsync(user);

            // Pobierz specjalizację na podstawie nazwy
            var specialization = await _specializationsRepository.GetSpecializationByNameAsync(dto.SpecializationName);
            if (specialization == null)
            {
                throw new Exception("Nie znaleziono takiej specjalizacji");
            }

            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                LicenseNumber = dto.LicenseNumber,
                SpecializationId = specialization.Id,
                UserId = user.Id
            };

            doctor.UserId = user.Id;

            await _doctorRepository.CreateDoctorAsync(doctor);
        }
        public async Task DeleteDoctorAsync(Guid DoctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(DoctorId);

            if (doctor != null)
            {
                var user = await _userRepository.GetUserByIdAsync(doctor.UserId);

                await _doctorRepository.DeleteDoctorAsync(doctor.Id);

                if (user != null)
                {
                    await _userRepository.DeleteUserAsync(user.Id);
                }
            }
        }
    }
}
