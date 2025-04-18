using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("addresses")]
    public class Address : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string FullName { get; set; }

        [Phone]
        public required string PhoneNumber { get; set; }

        public required string AddressDetail { get; set; }

        public required string Ward { get; set; }

        public required string District { get; set; }

        public required string City { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public required User User { get; set; }

        public bool Active { get; set; }


    }

}
