using SaboreIA.Models;
using System.ComponentModel.DataAnnotations;

namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO para atualização de dados do usuário
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Nome do usuário
        /// </summary>
        [MaxLength(150, ErrorMessage = "Nome não pode exceder 150 caracteres")]
        public string? Name { get; set; }

        /// <summary>
        /// Email do usuário
        /// </summary>
        [MaxLength(150, ErrorMessage = "Email não pode exceder 150 caracteres")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }

        /// <summary>
        /// Nova senha do usuário
        /// </summary>
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        [MaxLength(100, ErrorMessage = "Senha não pode exceder 100 caracteres")]
        public string? Password { get; set; }

        /// <summary>
        /// Telefone do usuário
        /// </summary>
        [MaxLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres")]
        public string? Phone { get; set; }

        /// <summary>
        /// URL da imagem de perfil
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Status de ativação do usuário
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Dados do endereço para atualização
        /// </summary>
        public UpdateAddressDto? Address { get; set; }
    }

    /// <summary>
    /// DTO de retorno com dados do usuário
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// ID único do usuário
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Telefone do usuário
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// URL da imagem de perfil
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Papel/função do usuário (USER, OWNER, ADMIN)
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Indica se o usuário está ativo
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Data de criação do usuário (UTC-3)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Endereço do usuário
        /// </summary>
        public AddressDto? Address { get; set; }
    }
}
