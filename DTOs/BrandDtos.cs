using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class BrandCreateDto
    {
        public required string Name { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class BrandResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
    }
}