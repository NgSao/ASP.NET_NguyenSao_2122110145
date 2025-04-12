using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("images")]
    public class Image : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string ImageUrl { get; set; }

        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}