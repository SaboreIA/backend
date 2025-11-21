using System.ComponentModel.DataAnnotations;

namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO de retorno com dados do favorito
    /// </summary>
    public class FavoriteDto
    {
        /// <summary>
        /// ID único do favorito
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ID do usuário que favoritou
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// ID do restaurante favoritado
        /// </summary>
        public long RestaurantId { get; set; }

        /// <summary>
        /// Data em que o favorito foi criado (UTC-3)
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Nome do restaurante favoritado
        /// </summary>
        public string RestaurantName { get; set; }

        /// <summary>
        /// URL da imagem de capa do restaurante
        /// </summary>
        public string? RestaurantCoverImageUrl { get; set; }

        /// <summary>
        /// Descrição do restaurante
        /// </summary>
        public string? RestaurantDescription { get; set; }
    }

    /// <summary>
    /// DTO para adicionar restaurante aos favoritos
    /// </summary>
    public class CreateFavoriteDto
    {
        /// <summary>
        /// ID do restaurante a ser favoritado
        /// </summary>
        [Required(ErrorMessage = "RestaurantId é obrigatório")]
        public long RestaurantId { get; set; }
    }

    /// <summary>
    /// DTO com status do favorito do usuário para um restaurante
    /// </summary>
    public class FavoriteStatusDto
    {
        /// <summary>
        /// Indica se o restaurante está nos favoritos do usuário
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// ID do registro de favorito (se existir)
        /// </summary>
        public long? FavoriteId { get; set; }
    }
}
