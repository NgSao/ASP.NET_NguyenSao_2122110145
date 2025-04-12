using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class ProductSpecificationCreateDto
    {
        public required int ProductId { get; set; }

        public required string Key { get; set; }

        public required string Value { get; set; }
    }

    public class ProductSpecificationResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}