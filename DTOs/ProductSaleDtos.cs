using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class ProductSaleCreateDto
    {
        public required int ColorId { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }

        public required DateTime StartDate { get; set; }

        public required DateTime EndDate { get; set; }
    }

    public class ProductSaleResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public int ColorId { get; set; }
        public string? ProductName { get; set; }
        public string? ColorName { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}