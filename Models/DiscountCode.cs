using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("discount_codes")]
    public class DiscountCode : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public required string Code { get; set; }

        [Range(0, 100)]
        public decimal? DiscountPercent { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue)]
        public int UsageLimit { get; set; }

        public int UsedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Order>? Orders { get; set; }
    }
}