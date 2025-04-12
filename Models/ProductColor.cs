using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("product_colors")]
    public class ProductColor : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string ColorName { get; set; }

        public string? ImageUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, int.MaxValue)]
        public int Sold { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OriginalPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal SalePrice { get; set; }

        public int VariantId { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }

        public virtual ICollection<ProductSale>? Sales { get; set; }
    }
}