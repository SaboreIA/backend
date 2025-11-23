using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaboreIA.Models
{
    [Table("Address")]
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string Street { get; set; }

        [Required]
        [MaxLength(20)]
        public string Number { get; set; }

        [MaxLength(100)]
        public string? Complement { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Se Address tem única relação 1:1 com User
        public virtual User? User { get; set; }

        // Inicializa a coleção para evitar nulls
        public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}
