using System.ComponentModel.DataAnnotations;

namespace SaboreIA.DTOs
{
    /// <summary>
    /// DTO para criação de novo endereço
    /// </summary>
    public class CreateAddressDto
    {
        /// <summary>
        /// CEP do endereço
        /// </summary>
        [Required(ErrorMessage = "CEP é obrigatório")]
        [MaxLength(20, ErrorMessage = "CEP não pode exceder 20 caracteres")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Nome da rua/avenida
        /// </summary>
        [Required(ErrorMessage = "Rua é obrigatória")]
        [MaxLength(255, ErrorMessage = "Rua não pode exceder 255 caracteres")]
        public string Street { get; set; }

        /// <summary>
        /// Número do endereço
        /// </summary>
        [Required(ErrorMessage = "Número é obrigatório")]
        [MaxLength(20, ErrorMessage = "Número não pode exceder 20 caracteres")]
        public string Number { get; set; }

        /// <summary>
        /// Complemento do endereço (opcional)
        /// </summary>
        [MaxLength(100, ErrorMessage = "Complemento não pode exceder 100 caracteres")]
        public string? Complement { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        [Required(ErrorMessage = "Cidade é obrigatória")]
        [MaxLength(100, ErrorMessage = "Cidade não pode exceder 100 caracteres")]
        public string City { get; set; }

        /// <summary>
        /// Estado/UF
        /// </summary>
        [Required(ErrorMessage = "Estado é obrigatório")]
        [MaxLength(50, ErrorMessage = "Estado não pode exceder 50 caracteres")]
        public string State { get; set; }

        /// <summary>
        /// País
        /// </summary>
        [Required(ErrorMessage = "País é obrigatório")]
        [MaxLength(50, ErrorMessage = "País não pode exceder 50 caracteres")]
        public string Country { get; set; }
    }

    /// <summary>
    /// DTO para atualização de endereço existente
    /// </summary>
    public class UpdateAddressDto
    {
        /// <summary>
        /// CEP do endereço
        /// </summary>
        [MaxLength(20, ErrorMessage = "CEP não pode exceder 20 caracteres")]
        public string? ZipCode { get; set; }

        /// <summary>
        /// Nome da rua/avenida
        /// </summary>
        [MaxLength(255, ErrorMessage = "Rua não pode exceder 255 caracteres")]
        public string? Street { get; set; }

        /// <summary>
        /// Número do endereço
        /// </summary>
        [MaxLength(20, ErrorMessage = "Número não pode exceder 20 caracteres")]
        public string? Number { get; set; }

        /// <summary>
        /// Complemento do endereço
        /// </summary>
        [MaxLength(100, ErrorMessage = "Complemento não pode exceder 100 caracteres")]
        public string? Complement { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        [MaxLength(100, ErrorMessage = "Cidade não pode exceder 100 caracteres")]
        public string? City { get; set; }

        /// <summary>
        /// Estado/UF
        /// </summary>
        [MaxLength(50, ErrorMessage = "Estado não pode exceder 50 caracteres")]
        public string? State { get; set; }

        /// <summary>
        /// País
        /// </summary>
        [MaxLength(50, ErrorMessage = "País não pode exceder 50 caracteres")]
        public string? Country { get; set; }
    }

    /// <summary>
    /// DTO de retorno com dados do endereço
    /// </summary>
    public class AddressDto
    {
        /// <summary>
        /// ID único do endereço
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// CEP do endereço
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Nome da rua/avenida
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Número do endereço
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Complemento do endereço
        /// </summary>
        public string? Complement { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Estado/UF
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// País
        /// </summary>
        public string Country { get; set; }
    }
}
