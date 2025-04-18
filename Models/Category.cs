using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("categories")]
    public class Category : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Slug { get; set; }

        public string? ImageUrl { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Category? Parent { get; set; }

        public required Status Status { get; set; } = Status.Active;

        public virtual ICollection<Category>? Children { get; set; } = new List<Category>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}