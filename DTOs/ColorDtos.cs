using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class ColorCreateDto
    {
        public required string ColorName { get; set; }
        public string? ImageUrl { get; set; }

        public required int Stock { get; set; }

        public required decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }

        public required int VariantId { get; set; }
        public MediaResponseDto? Media { get; set; }

    }

    public class ColorUpdateDto
    {
        public string? ColorName { get; set; }
        public string? ImageUrl { get; set; }

        public int Stock { get; set; }

        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }

        public int VariantId { get; set; }
        public MediaResponseDto? Media { get; set; }

    }

    public class ColorResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? ColorName { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int VariantId { get; set; }

        public MediaResponseDto? Media { get; set; }

    }
}