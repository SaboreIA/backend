using System.ComponentModel.DataAnnotations;

namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO para criação de nova tag/categoria
    /// </summary>
    public class CreateTagDto
    {
        /// <summary>
        /// Nome da tag/categoria (ex: "Pizza", "Italiana", "Vegana")
        /// </summary>
        [Required(ErrorMessage = "Nome da tag é obrigatório")]
        [MaxLength(50, ErrorMessage = "Nome não pode exceder 50 caracteres")]
        public string Name { get; set; }
    }

    /// <summary>
    /// DTO para atualização de tag existente
    /// </summary>
    public class UpdateTagDto
    {
        /// <summary>
        /// Novo nome da tag/categoria
        /// </summary>
        [MaxLength(50, ErrorMessage = "Nome não pode exceder 50 caracteres")]
        public string? Name { get; set; }
    }

    /// <summary>
    /// DTO de retorno com dados da tag
    /// </summary>
    public class TagDto
    {
        /// <summary>
        /// ID único da tag
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nome da tag/categoria
        /// </summary>
        public string Name { get; set; }
    }
}
