using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SaboreIA.DTOs
{
    // DTO para requisição de login do usuário
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Password { get; set; }
    }

    // DTO para requisição de registro de novo usuário
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(150, ErrorMessage = "Nome não pode exceder 150 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(150, ErrorMessage = "Email não pode exceder 150 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }

        [MaxLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres")]
        public string? Phone { get; set; }

        public string? ImageUrl { get; set; }

        public CreateAddressDto? Address { get; set; }
    }

    // DTO para resposta de autenticação (login/registro)
    public class AuthResponseDto
    {
        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public UserDto User { get; set; }
    }

    // DTO para representar o usuário autenticado
    public class CurrentUserDto
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        // Mapeia os claims do usuário autenticado para o DTO
        public static CurrentUserDto FromClaims(ClaimsPrincipal user)
        {
            return new CurrentUserDto
            {
                UserId = user.FindFirst("userId")?.Value ?? string.Empty,
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                Name = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
            };
        }
    }

}
