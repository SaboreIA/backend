using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaboreIA.Models
{
    [Table("Review")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating1 { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating2 { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating3 { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating4 { get; set; }

        public double AvgRating { get; set; }

        // Foreign Keys
        [Required]
        public long UserId { get; set; }

        [Required]
        public long RestaurantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("RestaurantId")]
        public virtual Restaurant Restaurant { get; set; }
    }
}
