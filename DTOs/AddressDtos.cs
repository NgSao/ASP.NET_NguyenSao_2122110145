using System.ComponentModel.DataAnnotations;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.DTOs
{
    public class AddressCreateDto
    {


        public required string FullName { get; set; }
        [Phone]
        public required string PhoneNumber { get; set; }

        public required string AddressDetail { get; set; }

        public required string Ward { get; set; }

        public required string District { get; set; }

        public required string City { get; set; }

        public bool Active { get; set; }

    }

    public class AddressUpdateDto
    {
        public string? FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? AddressDetail { get; set; }

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }


        public bool Active { get; set; }

    }


}