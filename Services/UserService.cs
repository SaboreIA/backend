using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Models;
using SaboreIA.Interfaces.Service;
using SaboreIA.Helpers;

namespace SaboreIA.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(IUserRepository userRepository, ICloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<UserDto?> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> UpdateAsync(long id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            // Atualizar apenas campos não nulos
            if (!string.IsNullOrWhiteSpace(updateUserDto.Name))
                user.Name = updateUserDto.Name;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
            {
                if (user.Email != updateUserDto.Email &&
                    await _userRepository.EmailExistsAsync(updateUserDto.Email))
                {
                    throw new InvalidOperationException("Email already exists.");
                }
                user.Email = updateUserDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Phone))
                user.Phone = updateUserDto.Phone;

            if (updateUserDto.Active.HasValue)
                user.Active = updateUserDto.Active.Value;

            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapToDto(updatedUser);
        }

        public async Task<UserDto?> UpdateUserImageAsync(long id, IFormFile imageFile)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            // Fazer upload da nova imagem para o Cloudinary
            var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "UserImages");

            // Atualizar a URL da imagem no usuário
            user.ImageUrl = imageUrl;
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                ImageUrl = user.ImageUrl,
                Role = user.Role,
                Active = user.Active,
                CreatedAt = DateTimeHelper.ToSaoPauloTime(user.CreatedAt),
                Address = MapAddressToDto(user.Address ?? null)
            };
        }

        private AddressDto MapAddressToDto(Address? address)
        {
            if (address == null)
                return null;

            return new AddressDto
            {
                Id = address.Id,
                ZipCode = address.ZipCode,
                Street = address.Street,
                Number = address.Number,
                Complement = address.Complement ?? null,
                City = address.City,
                State = address.State,
                Country = address.Country
            };
        }
    }
}
