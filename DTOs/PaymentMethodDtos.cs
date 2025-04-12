using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class PaymentMethodCreateDto
    {
        public required string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PaymentMethodResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }

    }
}