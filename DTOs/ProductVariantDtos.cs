using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class ProductVariantCreateDto
    {
        public required string Storage { get; set; }

        public required decimal BasePrice { get; set; }

        public required int ProductId { get; set; }
    }

    public class ProductVariantResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Storage { get; set; }
        public decimal BasePrice { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
    }
}