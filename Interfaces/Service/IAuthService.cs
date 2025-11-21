using SaboreIA.DTOs;

namespace SaboreIA.Interfaces.Service
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto);
    }
}
