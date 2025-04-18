using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("inventories")]
    public class Inventory : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int ColorId { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [ForeignKey("ColorId")]
        public virtual Color Color { get; set; } = default!;
    }
}