using System.ComponentModel.DataAnnotations;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.DTOs
{
    public class UserCreateDto
    {
        public required string Name { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required string Password { get; set; }

        public required RoleType Role { get; set; }
    }

    public class UserResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public RoleType Role { get; set; }
        public UserStatus UserStatus { get; set; }

    }
}