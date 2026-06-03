using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.EntityFrameworkCore;


namespace MedicalCenter.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public PatientService(IPatientRepository patientRepository, IUserRepository userRepository, IAppointmentRepository appointmentRepository)
        {
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<PatientDto>> GetAllPatientsAsync(int skip = 0, int take = int.MaxValue)
        {
            var patients = await _patientRepository.GetAllPatientsAsync(skip, take);
            var patientDtos = patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Phone = p.User.Phone,
                Pesel = p.Pesel
            }).ToList();

            return patientDtos;
        }
        public async Task<int> GetPatientsCountAsync()
        {
            return await _patientRepository.GetPatientsCountAsync();
        }

        public async Task RegisterAsync(PatientRegisterDto dto)
        {
            if (dto.BirthDate > DateTime.Today.AddYears(-13))
                throw new Exception("Pacjent musi mieć ukończone 13 lat.");

            if (dto.BirthDate < DateTime.Today.AddYears(-120))
                throw new Exception("Nieprawidłowa data urodzenia.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                PasswordHash = passwordHash,
                RoleId = 3
            };

            await _userRepository.CreateUserAsync(user);

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                BirthDate = dto.BirthDate,
                Pesel = dto.Pesel,
                Address = new Address()
            };

            patient.UserId = user.Id;

            await _patientRepository.CreatePatientAsync(patient);
        }
        public async Task RegisterGoogleUserAsync(string email, string firstName, string lastName)
        {
            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = null, // Google user doesn't have a password
                RoleId = 3
            };

            await _userRepository.CreateUserAsync(user);

            var patient = new Patient
            {
                UserId= user.Id,
                Address = new Address()
            };

            await _patientRepository.CreatePatientAsync(patient);
        }

        public async Task<PatientDto> GetPatientByIdAsync(Guid id)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id) ?? throw new Exception("Nie znaleziono Pacjenta.");

            var patientDto = new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.User.FirstName,
                LastName = patient.User.LastName,
                Phone = patient.User.Phone,
                Pesel = patient.Pesel
            };
            return patientDto;
        }

        public async Task<PatientDto> GetPatientByUserIdAsync(Guid userId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId) ?? throw new Exception("Nie znaleziono Pacjenta.");

            if (patient.User == null)
                throw new Exception("Pacjent nie ma przypisanego konta użytkownika.");

            var patientDto = new PatientDto
            {
                Id = patient.Id,
                Pesel = patient.Pesel,
                FirstName = patient.User.FirstName,
                LastName = patient.User.LastName,
                Phone = patient.User.Phone,
            };

            return patientDto;
        }
        public async Task<PatientProfileDto> GetPatientProfileAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception("Nie znaleziono użytkownika.");

            var patient = await _patientRepository.GetPatientByUserIdAsync(id) ?? throw new Exception("Nie znaleziono pacjenta.");

            var patientProfileDto = new PatientProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                ProfilePicturePath = user.ProfilePicturePath,
                Pesel = patient.Pesel,
                BirthDate = patient.BirthDate
            };
            return patientProfileDto;
        }

        public async Task UpdatePatientProfileAsync(Guid id, UpdatePatientProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception("Nie znaleziono użytkownika.");
            var patient = await _patientRepository.GetPatientByUserIdAsync(id) ?? throw new Exception("Nie znaleziono pacjenta.");

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Email = dto.Email ?? user.Email;
            patient.Pesel = dto.Pesel ?? patient.Pesel;
            if (dto.BirthDate.HasValue)
                patient.BirthDate = dto.BirthDate.Value;

            await _userRepository.UpdateUserAsync(user);
            await _patientRepository.UpdatePatientAsync(patient);
        }

        public async Task DeletePatientAsync(Guid patientId)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(patientId) ?? throw new Exception("Nie znaleziono pacjenta.");
            var user = await _userRepository.GetUserByIdAsync(patient.UserId) ?? throw new Exception("Nie znaleziono użytkownika.");

            await _appointmentRepository.DeleteAppointmentsByPatientIdAsync(patientId);
            await _patientRepository.DeletePatientAsync(patientId);
            await _userRepository.DeleteUserAsync(patient.UserId);
        }
        public async Task<List<PatientDemographicsDto>> GetPatientDemographicsAsync(int ageFrom, int ageTo)
        {
            return await _patientRepository.GetPatientDemographicsAsync(ageFrom, ageTo);
        }
    }
}
