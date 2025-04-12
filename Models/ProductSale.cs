using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("product_sales")]
    public class ProductSale : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int ProductColorId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountAmount { get; set; }

        [Range(0, 100)]
        public decimal? DiscountPercent { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey("ProductColorId")]
        public virtual ProductColor? ProductColor { get; set; }
    }
}