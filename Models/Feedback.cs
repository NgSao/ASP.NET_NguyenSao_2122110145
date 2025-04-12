using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("feedbacks")]
    public class Feedback : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required int QuestionId { get; set; }

        [StringLength(1000)]
        public required string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question? Question { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}