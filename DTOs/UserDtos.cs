using System.ComponentModel.DataAnnotations;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.DTOs
{
    public class UserCreateDto
    {
        public required string FullName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required string Password { get; set; }

        public required RoleStatus Role { get; set; }
    }

    public class UserUpdateDto
    {
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public RoleStatus Role { get; set; }
        public UserStatus UserStatus { get; set; }
    }


    public class UserResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public RoleStatus Role { get; set; }
        public UserStatus UserStatus { get; set; }
        public List<AddressResponseDto>? Addresses { get; set; }
    }
    public class AddressResponseDto
    {
        public int Id { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? AddressDetail { get; set; }

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }
        public bool Active { get; set; }



    }
}