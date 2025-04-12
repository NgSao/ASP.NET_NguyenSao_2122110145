using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NguyenSao_2122110145.Models
{
    [Table("products")]
    [Index(nameof(CategoryId), nameof(BrandId))]
    public class Product : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand? Brand { get; set; }

        public IEnumerable<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        public virtual IEnumerable<Image>? Images { get; set; }
    }
}
