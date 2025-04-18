using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("order_details")]
    public class OrderDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ColorId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ColorId")]
        public virtual Color? Color { get; set; }
    }
}