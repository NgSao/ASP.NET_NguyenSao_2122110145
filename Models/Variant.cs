using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("variants")]
    public class Variant : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public required string Size { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public required Product Product { get; set; }

        public virtual IEnumerable<Color> Colors { get; set; } = new List<Color>();
    }
}