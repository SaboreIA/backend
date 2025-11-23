using System.ComponentModel.DataAnnotations;

namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO de retorno com dados da avaliação
    /// </summary>
    public class ReviewDto
    {
        /// <summary>
        /// ID único da avaliação
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Título da avaliação
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Comentário detalhado da avaliação
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// URL da imagem anexada à avaliação
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Nota para qualidade da comida (1-5)
        /// </summary>
        public int Rating1 { get; set; }

        /// <summary>
        /// Nota para atendimento (1-5)
        /// </summary>
        public int Rating2 { get; set; }

        /// <summary>
        /// Nota para ambiente (1-5)
        /// </summary>
        public int Rating3 { get; set; }

        /// <summary>
        /// Nota para custo-benefício (1-5)
        /// </summary>
        public int Rating4 { get; set; }

        /// <summary>
        /// Média das 4 notas
        /// </summary>
        public double AvgRating { get; set; }

        /// <summary>
        /// ID do usuário que fez a avaliação
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// ID do restaurante avaliado
        /// </summary>
        public long RestaurantId { get; set; }

        /// <summary>
        /// Data de criação da avaliação (UTC-3)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data da última atualização (UTC-3)
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Nome do usuário que fez a avaliação
        /// </summary>
        public string UserName { get; set; }
    }

    /// <summary>
    /// DTO para criação de nova avaliação
    /// </summary>
    public class CreateReviewDto
    {
        /// <summary>
        /// Título da avaliação
        /// </summary>
        [Required(ErrorMessage = "Título é obrigatório")]
        [MaxLength(255, ErrorMessage = "Título não pode exceder 255 caracteres")]
        public string Title { get; set; }

        /// <summary>
        /// Comentário detalhado sobre a experiência
        /// </summary>
        [Required(ErrorMessage = "Comentário é obrigatório")]
        [MaxLength(1000, ErrorMessage = "Comentário não pode exceder 1000 caracteres")]
        public string Comment { get; set; }

        /// <summary>
        /// URL da imagem anexada
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Nota para qualidade da comida (1-5)
        /// </summary>
        [Required(ErrorMessage = "Rating1 é obrigatório")]
        [Range(1, 5, ErrorMessage = "Rating deve ser entre 1 e 5")]
        public int Rating1 { get; set; }

        /// <summary>
        /// Nota para atendimento (1-5)
        /// </summary>
        [Required(ErrorMessage = "Rating2 é obrigatório")]
        [Range(1, 5, ErrorMessage = "Rating deve ser entre 1 e 5")]
        public int Rating2 { get; set; }

        /// <summary>
        /// Nota para ambiente (1-5)
        /// </summary>
        [Required(ErrorMessage = "Rating3 é obrigatório")]
        [Range(1, 5, ErrorMessage = "Rating deve ser entre 1 e 5")]
        public int Rating3 { get; set; }

        /// <summary>
        /// Nota para custo-benefício (1-5)
        /// </summary>
        [Required(ErrorMessage = "Rating4 é obrigatório")]
        [Range(1, 5, ErrorMessage = "Rating deve ser entre 1 e 5")]
        public int Rating4 { get; set; }

        /// <summary>
        /// ID do restaurante sendo avaliado
        /// </summary>
        [Required(ErrorMessage = "RestaurantId é obrigatório")]
        public long RestaurantId { get; set; }
    }

    /// <summary>
    /// DTO para atualização de avaliação existente
    /// </summary>
    public class UpdateReviewDto
    {
        /// <summary>
        /// Novo título da avaliação
        /// </summary>
        [MaxLength(255, ErrorMessage = "Título não pode exceder 255 caracteres")]
        public string? Title { get; set; }

        /// <summary>
        /// Novo comentário
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Comentário não pode exceder 1000 caracteres")]
        public string? Comment { get; set; }

        /// <summary>
        /// Nova URL da imagem
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Nova nota para qualidade da comida (1-5)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating deve ser entre 1 e 5")]
        public int? Rating1 { get; set; }

        /// <summary>
        /// Nova nota para atendimento (1-5)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating deve ser entre 1 e 5")]
        public int? Rating2 { get; set; }

        /// <summary>
        /// Nova nota para ambiente (1-5)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating deve ser entre 1 e 5")]
        public int? Rating3 { get; set; }

        /// <summary>
        /// Nova nota para custo-benefício (1-5)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating deve ser entre 1 e 5")]
        public int? Rating4 { get; set; }
    }
}
