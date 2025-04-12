using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.Models
{
    [Table("order_details")]
    public class OrderDetail : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
