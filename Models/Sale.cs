using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("sales")]
    public class Sale : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int ColorId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountAmount { get; set; }

        [Range(0, 100)]
        public decimal? DiscountPercent { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey("ColorId")]
        public required Color Color { get; set; }

        public required Status Status { get; set; } = Status.Active;

    }
}