using System.ComponentModel.DataAnnotations;

namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO para pesquisa de restaurantes usando IA
    /// </summary>
    public class SearchRestaurantDto
    {
        /// <summary>
        /// Entrada de texto do usuário para busca (ex: "pizza", "comida italiana")
        /// </summary>
        [Required(ErrorMessage = "Entrada do usuário é obrigatória")]
        [MaxLength(200, ErrorMessage = "Entrada não pode exceder 200 caracteres")]
        public string UserInput { get; set; }
    }

    /// <summary>
    /// DTO para criação de novo restaurante
    /// </summary>
    public class CreateRestaurantDto
    {
        /// <summary>
        /// Nome do restaurante
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(255, ErrorMessage = "Nome não pode exceder 255 caracteres")]
        public string Name { get; set; }

        /// <summary>
        /// Telefone de contato do restaurante
        /// </summary>
        [MaxLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Email de contato do restaurante
        /// </summary>
        [MaxLength(150, ErrorMessage = "Email não pode exceder 150 caracteres")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }

        /// <summary>
        /// Descrição do restaurante
        /// </summary>
        [MaxLength(500, ErrorMessage = "Descrição não pode exceder 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// URL do site do restaurante
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL do site não pode exceder 255 caracteres")]
        public string? Site { get; set; }

        /// <summary>
        /// URL ou descrição do cardápio
        /// </summary>
        public string? Menu { get; set; }

        /// <summary>
        /// URL da imagem de capa
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL da imagem não pode exceder 255 caracteres")]
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// URL da primeira imagem adicional
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL da imagem não pode exceder 255 caracteres")]
        public string? ImageUrl1 { get; set; }

        /// <summary>
        /// URL da segunda imagem adicional
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL da imagem não pode exceder 255 caracteres")]
        public string? ImageUrl2 { get; set; }

        /// <summary>
        /// URL da terceira imagem adicional
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL da imagem não pode exceder 255 caracteres")]
        public string? ImageUrl3 { get; set; }

        /// <summary>
        /// Endereço do restaurante
        /// </summary>
        [Required(ErrorMessage = "Endereço é obrigatório")]
        public CreateAddressDto Address { get; set; }

        /// <summary>
        /// Dia de abertura (0=Domingo, 1=Segunda, ..., 6=Sábado)
        /// </summary>
        [Range(0, 6, ErrorMessage = "Dia de abertura deve estar entre 0 (Domingo) e 6 (Sábado)")]
        public int OpenDay { get; set; }

        /// <summary>
        /// Dia de fechamento (0=Domingo, 1=Segunda, ..., 6=Sábado)
        /// </summary>
        [Range(0, 6, ErrorMessage = "Dia de fechamento deve estar entre 0 (Domingo) e 6 (Sábado)")]
        public int CloseDay { get; set; }

        /// <summary>
        /// Horário de abertura (formato: HH:mm)
        /// </summary>
        public string OpenTime { get; set; }

        /// <summary>
        /// Horário de fechamento (formato: HH:mm)
        /// </summary>
        public string CloseTime { get; set; }

        /// <summary>
        /// IDs das tags/categorias do restaurante
        /// </summary>
        public List<long> TagIds { get; set; } = new();
    }

    /// <summary>
    /// DTO para atualização de restaurante existente
    /// </summary>
    public class UpdateRestaurantDto
    {
        /// <summary>
        /// Nome do restaurante
        /// </summary>
        [MaxLength(255, ErrorMessage = "Nome não pode exceder 255 caracteres")]
        public string? Name { get; set; }

        /// <summary>
        /// Telefone de contato
        /// </summary>
        [MaxLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Email de contato
        /// </summary>
        [MaxLength(150, ErrorMessage = "Email não pode exceder 150 caracteres")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }

        /// <summary>
        /// Descrição do restaurante
        /// </summary>
        [MaxLength(500, ErrorMessage = "Descrição não pode exceder 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// URL do site
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL do site não pode exceder 255 caracteres")]
        public string? Site { get; set; }

        /// <summary>
        /// URL ou descrição do cardápio
        /// </summary>
        public string? Menu { get; set; }

        /// <summary>
        /// URL da imagem de capa
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL da imagem não pode exceder 255 caracteres")]
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// URL da primeira imagem adicional
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL da imagem não pode exceder 255 caracteres")]
        public string? ImageUrl1 { get; set; }

        /// <summary>
        /// URL da segunda imagem adicional
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL da imagem não pode exceder 255 caracteres")]
        public string? ImageUrl2 { get; set; }

        /// <summary>
        /// URL da terceira imagem adicional
        /// </summary>
        [MaxLength(255, ErrorMessage = "URL da imagem não pode exceder 255 caracteres")]
        public string? ImageUrl3 { get; set; }

        /// <summary>
        /// Status de ativação do restaurante
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Dia de abertura (0=Domingo, 1=Segunda, ..., 6=Sábado)
        /// </summary>
        [Range(0, 6, ErrorMessage = "Dia de abertura deve estar entre 0 (Domingo) e 6 (Sábado)")]
        public int? OpenDay { get; set; }

        /// <summary>
        /// Dia de fechamento (0=Domingo, 1=Segunda, ..., 6=Sábado)
        /// </summary>
        [Range(0, 6, ErrorMessage = "Dia de fechamento deve estar entre 0 (Domingo) e 6 (Sábado)")]
        public int? CloseDay { get; set; }

        /// <summary>
        /// Horário de abertura (formato: HH:mm)
        /// </summary>
        public string? OpenTime { get; set; } 

        /// <summary>
        /// Horário de fechamento (formato: HH:mm)
        /// </summary>
        public string? CloseTime { get; set; }  

        /// <summary>
        /// Dados do endereço para atualização
        /// </summary>
        public UpdateAddressDto? Address { get; set; }

        /// <summary>
        /// IDs das tags/categorias do restaurante
        /// </summary>
        public List<long>? TagIds { get; set; }
    }

    /// <summary>
    /// DTO para listagem resumida de restaurantes
    /// </summary>
    public class RestaurantListDTO
    {
        /// <summary>
        /// ID único do restaurante
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nome do restaurante
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email de contato
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Descrição do restaurante
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL da imagem de capa
        /// </summary>
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// URL da primeira imagem adicional
        /// </summary>
        public string? ImageUrl1 { get; set; }

        /// <summary>
        /// URL da segunda imagem adicional
        /// </summary>
        public string? ImageUrl2 { get; set; }

        /// <summary>
        /// URL da terceira imagem adicional
        /// </summary>
        public string? ImageUrl3 { get; set; }

        /// <summary>
        /// Indica se o restaurante está ativo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Lista de tags/categorias do restaurante
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// Cidade onde o restaurante está localizado
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Estado onde o restaurante está localizado
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Dia de abertura (0=Domingo, 1=Segunda, ..., 6=Sábado)
        /// </summary>
        public int OpenDay { get; set; }

        /// <summary>
        /// Dia de fechamento (0=Domingo, 1=Segunda, ..., 6=Sábado)
        /// </summary>
        public int CloseDay { get; set; }

        /// <summary>
        /// Horário de abertura
        /// </summary>
        public TimeSpan OpenTime { get; set; } 

        /// <summary>
        /// Horário de fechamento
        /// </summary>
        public TimeSpan CloseTime { get; set; }

        /// <summary>
        /// Média de avaliações (0-5)
        /// </summary>
        public double AvgRating { get; set; }

        /// <summary>
        /// Quantidade total de avaliações
        /// </summary>
        public int ReviewCount { get; set; } 
    }

    /// <summary>
    /// DTO com detalhes completos do restaurante
    /// </summary>
    public class RestaurantDetailDto
    {
        /// <summary>
        /// ID único do restaurante
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nome do restaurante
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Telefone de contato
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Email de contato
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Descrição do restaurante
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL do site
        /// </summary>
        public string? Site { get; set; }

        /// <summary>
        /// URL ou descrição do cardápio
        /// </summary>
        public string? Menu { get; set; }

        /// <summary>
        /// URL da imagem de capa
        /// </summary>
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// URL da primeira imagem adicional
        /// </summary>
        public string? ImageUrl1 { get; set; }

        /// <summary>
        /// URL da segunda imagem adicional
        /// </summary>
        public string? ImageUrl2 { get; set; }

        /// <summary>
        /// URL da terceira imagem adicional
        /// </summary>
        public string? ImageUrl3 { get; set; }

        /// <summary>
        /// Indica se o restaurante está ativo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Endereço completo do restaurante
        /// </summary>
        public AddressDto Address { get; set; }

        /// <summary>
        /// Tags/categorias do restaurante
        /// </summary>
        public List<TagDto> Tags { get; set; } = new();

        /// <summary>
        /// Nome do proprietário do restaurante
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// Nome do proprietário do restaurante
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// Data de criação do registro (UTC-3)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Dia de abertura (0=Domingo, 1=Segunda, ..., 6=Sábado)
        /// </summary>
        public int OpenDay { get; set; }

        /// <summary>
        /// Dia de fechamento (0=Domingo, 1=Segunda, ..., 6=Sábado)
        /// </summary>
        public int CloseDay { get; set; }

        /// <summary>
        /// Horário de abertura
        /// </summary>
        public TimeSpan OpenTime { get; set; } 

        /// <summary>
        /// Horário de fechamento
        /// </summary>
        public TimeSpan CloseTime { get; set; }

        /// <summary>
        /// Média de avaliações (0-5)
        /// </summary>
        public double AvgRating { get; set; }

        /// <summary>
        /// Quantidade total de avaliações
        /// </summary>
        public int ReviewCount { get; set; }
    }
}
