using Humanizer;
using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;


namespace MedicalCenter.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;

        public PatientService(IPatientRepository patientRepository, IUserRepository userRepository)
        {
            _patientRepository = patientRepository;
            _userRepository = userRepository;
        }

        public async Task<List<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllPatientsAsync();
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

        public async Task RegisterAsync(PatientRegisterDto dto)
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
                RoleId = 3
            };

            await _userRepository.CreateUserAsync(user);

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                BirthDate = dto.BirthDate,
                Pesel = dto.Pesel
            };

            patient.UserId = user.Id;

            await _patientRepository.CreatePatientAsync(patient);
        }
        public async Task RegisterGoogleUserAsync(string email, string firstName, string lastName)
        {
            var user = new User
            {
                Email = email,
                FirstName = firstName ?? "Użytkownik",
                LastName = lastName ?? "Google",
                PasswordHash = null, // Google user doesn't have a password
                RoleId = 3
            };

            await _userRepository.CreateUserAsync(user);

            var patient = new Patient
            {
                UserId= user.Id
            };

            await _patientRepository.CreatePatientAsync(patient);
        }
        public async Task<PatientDto> GetPatientByUserIdAsync(Guid userId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);

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
        public async Task<PatientDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailWithRoleAsync(email);
            if (user == null)
            {
                return null;
            }

            var patient = await _patientRepository.GetPatientByUserIdAsync(user.Id);
            if (patient == null)
            {
                return null;
            }

            var patientDto = new PatientDto
            {
                Id = patient.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Pesel = patient.Pesel
            };

            return patientDto;
        }
        public async Task<UpdatePatientProfileDto> GetPatientProfileAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var patient = await _patientRepository.GetPatientByUserIdAsync(id);
            var patientProfileDto = new UpdatePatientProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                ProfilePicturePath = user.ProfilePicturePath,
                Pesel = patient.Pesel,
                BirthDate = patient.BirthDate,
            };
            return patientProfileDto;
        }

        public async Task UpdatePatientProfileAsync(Guid id, UpdatePatientProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var patient = await _patientRepository.GetPatientByUserIdAsync(id);

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
    }
}
