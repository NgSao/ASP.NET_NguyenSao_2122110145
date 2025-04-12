using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("questions")]
    public class Question : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [StringLength(500)]
        public required string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}