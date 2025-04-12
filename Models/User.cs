using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("users")]
    public class User : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Avatar { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public required RoleType Role { get; set; }

        public UserStatus? UserStatus { get; set; }

    }
}
