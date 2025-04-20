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
        public required string Slug { get; set; }

        public required string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OriginalPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal SalePrice { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public required Category Category { get; set; }

        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public required Brand Brand { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, int.MaxValue)]
        public int Sold { get; set; }

    }
}
