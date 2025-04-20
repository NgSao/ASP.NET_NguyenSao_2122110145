using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class BrandCreateDto
    {
        public required string Name { get; set; }

        public required IFormFile File { get; set; }
        public required string Slug { get; set; }

    }

    public class BrandUpdateDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }

        public IFormFile? File { get; set; }
    }

    public class BrandResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Slug { get; set; }

        public string? Name { get; set; }
        public string? ImageUrl { get; set; }

        public int totalProduct { get; set; }
    }
}