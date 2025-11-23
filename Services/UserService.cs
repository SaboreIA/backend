using SaboreIA.DTOs;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Models;
using SaboreIA.Integrations.Cloudinary;
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

        public async Task<PaginatedResponseDTO<UserDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetAllAsync();

            // Paginação
            var paginatedUsers = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            return new PaginatedResponseDTO<UserDto>
            {
                Items = paginatedUsers,
                TotalItems = users.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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

            // Atualizar endereço se fornecido
            if (updateUserDto.Address != null)
            {
                if (user.Address != null)
                {
                    // Atualizar endereço existente
                    if (!string.IsNullOrWhiteSpace(updateUserDto.Address.ZipCode))
                        user.Address.ZipCode = updateUserDto.Address.ZipCode;

                    if (!string.IsNullOrWhiteSpace(updateUserDto.Address.Street))
                        user.Address.Street = updateUserDto.Address.Street;

                    if (!string.IsNullOrWhiteSpace(updateUserDto.Address.Number))
                        user.Address.Number = updateUserDto.Address.Number;

                    if (updateUserDto.Address.Complement != null)
                        user.Address.Complement = updateUserDto.Address.Complement;

                    if (!string.IsNullOrWhiteSpace(updateUserDto.Address.City))
                        user.Address.City = updateUserDto.Address.City;

                    if (!string.IsNullOrWhiteSpace(updateUserDto.Address.State))
                        user.Address.State = updateUserDto.Address.State;

                    if (!string.IsNullOrWhiteSpace(updateUserDto.Address.Country))
                        user.Address.Country = updateUserDto.Address.Country;

                    user.Address.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Criar novo endereço
                    user.Address = new Address
                    {
                        ZipCode = updateUserDto.Address.ZipCode ?? string.Empty,
                        Street = updateUserDto.Address.Street ?? string.Empty,
                        Number = updateUserDto.Address.Number ?? string.Empty,
                        Complement = updateUserDto.Address.Complement,
                        City = updateUserDto.Address.City ?? string.Empty,
                        State = updateUserDto.Address.State ?? string.Empty,
                        Country = updateUserDto.Address.Country ?? string.Empty,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                }
            }

            user.UpdatedAt = DateTime.UtcNow;

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
