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

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}