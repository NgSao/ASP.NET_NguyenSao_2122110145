using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("Medias")]
    public class Media : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string ImageUrl { get; set; }

        public int? ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }


        public int? ColorId { get; set; }

        [ForeignKey("ColorId")]
        public Color? Color { get; set; }
    }
}