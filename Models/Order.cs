using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("orders")]
    public class Order : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int AddressId { get; set; }

        public int PaymentMethodId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal FinalAmount { get; set; }

        public int? DiscountCodeId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address? Address { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod? PaymentMethod { get; set; }

        [ForeignKey("DiscountCodeId")]
        public virtual DiscountCode? DiscountCode { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}