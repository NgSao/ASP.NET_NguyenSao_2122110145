using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class DiscountCodeCreateDto
    {
        public required string Code { get; set; }

        [Range(0, 100)]
        public decimal? DiscountPercent { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountAmount { get; set; }

        public required DateTime StartDate { get; set; }

        public required DateTime EndDate { get; set; }

        [Range(1, int.MaxValue)]
        public int UsageLimit { get; set; }
    }

    public class DiscountCodeResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
    }
}