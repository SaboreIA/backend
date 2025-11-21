using SaboreIA.DTOs;
using Microsoft.AspNetCore.Http;

namespace SaboreIA.Interfaces.Service
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(long id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> UpdateAsync(long id, UpdateUserDto updateUserDto);
        Task<UserDto?> UpdateUserImageAsync(long id, IFormFile imageFile);
        Task<bool> DeleteAsync(long id);
    }
}
