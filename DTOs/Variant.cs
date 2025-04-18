using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class VariantCreateDto
    {
        public required string Size { get; set; }

        public required decimal BasePrice { get; set; }

        public required int ProductId { get; set; }
    }

    public class VariantUpdateDto
    {
        public string? Size { get; set; }

        public decimal? BasePrice { get; set; }

        public int? ProductId { get; set; }
    }

    public class VariantResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Size { get; set; }
        public decimal BasePrice { get; set; }
        public int ProductId { get; set; }

        List<ColorResponseDto>? Colors { get; set; }
    }
}