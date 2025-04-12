using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("inventories")]
    public class Inventory : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int ProductColorId { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [ForeignKey("ProductColorId")]
        public virtual ProductColor? ProductColor { get; set; }
    }
}