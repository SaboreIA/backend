using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO para requisição de login do usuário
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO para requisição de registro de novo usuário
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(150, ErrorMessage = "Nome não pode exceder 150 caracteres")]
        public string Name { get; set; }

        /// <summary>
        /// Email do usuário (deve ser único)
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(150, ErrorMessage = "Email não pode exceder 150 caracteres")]
        public string Email { get; set; }

        /// <summary>
        /// Senha do usuário (mínimo 6 caracteres)
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }

        /// <summary>
        /// Telefone do usuário (opcional)
        /// </summary>
        [MaxLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres")]
        public string? Phone { get; set; }

        /// <summary>
        /// URL da imagem de perfil do usuário (opcional)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Endereço do usuário (opcional)
        /// </summary>
        public CreateAddressDto? Address { get; set; }
    }

    /// <summary>
    /// DTO de resposta para autenticação (login/registro)
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Token JWT para autenticação
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Data e hora de expiração do token (UTC-3)
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Dados do usuário autenticado
        /// </summary>
        public UserDto User { get; set; }
    }

    /// <summary>
    /// DTO com informações do usuário atual extraídas do token JWT
    /// </summary>
    public class CurrentUserDto
    {
        /// <summary>
        /// ID do usuário
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Papel/função do usuário (USER, OWNER, ADMIN)
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// URL da imagem de perfil do usuário
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Extrai informações do usuário a partir dos Claims do token JWT
        /// </summary>
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
