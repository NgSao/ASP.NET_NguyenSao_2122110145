using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class CartItemCreateDto
    {
        public required int ColorId { get; set; }

        [Range(1, int.MaxValue)]
        public required int Quantity { get; set; }
    }

    public class CartItemResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ColorId { get; set; }
        public string? ProductName { get; set; }
        public string? ColorName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}