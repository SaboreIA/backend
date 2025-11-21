using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaboreIA.Models
{
    [Table("Restaurant")]
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = "Undefined";

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? Site { get; set; }
        public string? Menu { get; set; }

        // 0=domingo, 1=segunda, etc.}
        public int OpenDay { get; set; }
        public int CloseDay { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        [MaxLength(255)]
        public string? CoverImageUrl { get; set; }
        public string? ImageUrl1 { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign Keys
        [Required]
        public long OwnerId { get; set; }

        [Required]
        public long AddressId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        public virtual ICollection<Tag> Categories { get; set; } = new List<Tag>();
    }
}
