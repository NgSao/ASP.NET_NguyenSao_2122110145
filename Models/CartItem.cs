using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("cart_items")]
    public class CartItem : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ColorId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [ForeignKey("UserId")]
        public required User User { get; set; }

        [ForeignKey("ColorId")]
        public required Color Color { get; set; }
    }
}