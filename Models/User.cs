using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("users")]
    public class User : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string FullName { get; set; }

        public string? Avatar { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public required RoleStatus Role { get; set; }

        public required UserStatus UserStatus { get; set; }

        public DateTime? LastLogin { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

    }
}
