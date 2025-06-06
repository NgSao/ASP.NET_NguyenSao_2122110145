using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class CategoryCreateDto
    {
        public required string Name { get; set; }

        public required string Slug { get; set; }
        public required IFormFile File { get; set; }


    }

    public class CategoryUpdateDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }

        public IFormFile? File { get; set; }
    }

    public class CategoryResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Slug { get; set; }

        public string? Name { get; set; }
        public string? ImageUrl { get; set; }

        public int totalProduct { get; set; }
    }
}