using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class InventoryUpdateDto
    {
        public required int ProductColorId { get; set; }

        [Range(0, int.MaxValue)]
        public required int Quantity { get; set; }
    }

    public class InventoryResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public int ProductColorId { get; set; }
        public string? ProductName { get; set; }
        public string? ColorName { get; set; }
        public int Quantity { get; set; }
    }
}