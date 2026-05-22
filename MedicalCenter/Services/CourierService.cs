using MedicalCenter.DTOs;
using MedicalCenter.Models;
using MedicalCenter.Repositories;

namespace MedicalCenter.Services
{
    public class CourierService : ICourierService
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;

        public CourierService(ICourierRepository courierRepository, IUserRepository userRepository, IOrderRepository orderRepository)
        {
            _courierRepository = courierRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        private CourierDto MapToCourierDto(Courier courier)
        {
            if (courier == null) return null;

            var courierDto = new CourierDto
            {
                Id = courier.Id,
                FirstName = courier.User?.FirstName ?? string.Empty,
                LastName = courier.User?.LastName ?? string.Empty,
                Phone = courier.User?.Phone ?? string.Empty,
                VehicleRegistration = courier.VehicleRegistration
            };
            return courierDto;
        }

        public async Task<List<CourierDto>> GetAllCourierAsync()
        {
            var couriers = await _courierRepository.GetAllCourierAsync();
            var courierDtos = new List<CourierDto>();
            foreach (var courier in couriers)
            {
                courierDtos.Add(MapToCourierDto(courier));
            }
            return courierDtos;
        }

        public async Task<CourierDto> GetCourierByIdAsync(Guid id)
        {
            var courier = await _courierRepository.GetCourierByIdAsync(id) ?? throw new Exception("Nie znaleziono kuriera.");
            return MapToCourierDto(courier);
        }

        public async Task<CourierDto> GetCourierByUserIdAsync(Guid userId)
        {
            var courier = await _courierRepository.GetCourierByUserIdAsync(userId);
            return MapToCourierDto(courier);
        }

        public async Task CreateCourierAsync(AdminCreateDto dto)
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
                RoleId = 4
            };

            await _userRepository.CreateUserAsync(user);

            var courier = new Courier
            {
                UserId = user.Id
            };

            await _courierRepository.CreateCourierAsync(courier);
        }

        public async Task DeleteCourierAsync(Guid CourierId)
        {
            var courier = await _courierRepository.GetCourierByIdAsync(CourierId);
            if (courier == null)
            {
                throw new Exception("Nie znaleziono kuriera.");
            }
            await _courierRepository.DeleteCourierAsync(CourierId);
        }

        public async Task<CourierProfileDto> GetCourierProfileAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception("Nie znaleziono użytkownika.");
            var courier = await _courierRepository.GetCourierByUserIdAsync(id) ?? throw new Exception("Nie znaleziono kuriera.");

            var courierProfileDto = new CourierProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                ProfilePicturePath = user.ProfilePicturePath,
                VehicleRegistration = courier.VehicleRegistration
            };
            return courierProfileDto;
        }

        public async Task UpdateCourierProfileAsync(Guid id, UpdateCourierProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id) ?? throw new Exception("Nie znaleziono użytkownika.");
            var courier = await _courierRepository.GetCourierByUserIdAsync(id) ?? throw new Exception("Nie znaleziono kuriera.");

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Email = dto.Email ?? user.Email;

            courier.VehicleRegistration = dto.VehicleRegistration ?? courier.VehicleRegistration;

            await _userRepository.UpdateUserAsync(user);
            await _courierRepository.UpdateCourierAsync(id, courier);
        }

        public async Task ChangeDeliveryStatusAsync(Guid deliveryId, int newStatusId)
        {
            var delivery = await _courierRepository.GetDeliveryByIdAsync(deliveryId);

            if (delivery != null)
            {
                delivery.StatusId = newStatusId;

                if (newStatusId == 4)
                {
                    delivery.DeliveredAt = DateTime.Now;
                }

                var order = await _orderRepository.GetOrderByIdAsync(delivery.OrderId);

                if (order != null)
                {
                    order.StatusId = newStatusId;
                }

                await _courierRepository.SaveChangesAsync();
            }
        }
    }
}