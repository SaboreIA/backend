using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaboreIA.Models
{
    public enum UserRole
    {
        USER,
        ADMIN
    }

    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }
        public string? ImageUrl { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.USER;

        public bool Active { get; set; } = true;

        public long? AddressId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        // Navigation properties
        public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}
