using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("payment_methods")]
    public class PaymentMethod : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public bool IsActive { get; set; } = true;
    }
}