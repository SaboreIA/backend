using System.ComponentModel.DataAnnotations;

namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO para criação de novo produto
    /// </summary>
    public class ProductCreateDto
    {
        /// <summary>
        /// Nome do produto (ex: "Pizza Margherita", "Espaguete Carbonara")
        /// </summary>
        [Required(ErrorMessage = "Nome do produto é obrigatório")]
        public string Name { get; set; }

        /// <summary>
        /// ID da tag/categoria associada ao produto
        /// </summary>
        [Required(ErrorMessage = "TagId é obrigatório")]
        public long? TagId { get; set; }
    }

    /// <summary>
    /// DTO para atualização de produto existente
    /// </summary>
    public class ProductUpdateDto
    {
        /// <summary>
        /// ID do produto a ser atualizado
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Novo nome do produto
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Novo ID da tag/categoria
        /// </summary>
        public long TagId { get; set; }
    }

    /// <summary>
    /// DTO de retorno com informações do produto
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// ID único do produto
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nome do produto
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// ID da tag/categoria associada
        /// </summary>
        public long TagId { get; set; }

        /// <summary>
        /// Nome da tag/categoria
        /// </summary>
        public string TagName { get; set; } = null!;
    }
}
