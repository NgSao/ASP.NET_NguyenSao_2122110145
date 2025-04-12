using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("products")]
    public class Product : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }


    }
}
