using System.ComponentModel.DataAnnotations;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.DTOs
{
    public class OrderCreateDto
    {
        public required int UserId { get; set; }

        public required int AddressId { get; set; }

        public required int PaymentMethodId { get; set; }

        public int? DiscountCodeId { get; set; }

        public required List<OrderDetailCreateDto> OrderDetails { get; set; }
    }

    public class OrderDetailCreateDto
    {
        public required int ColorId { get; set; }

        [Range(1, int.MaxValue)]
        public required int Quantity { get; set; }

        public required decimal Price { get; set; }
    }

    public class OrderResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int AddressId { get; set; }
        public string? AddressDetail { get; set; }
        public int PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public int? DiscountCodeId { get; set; }
        public string? DiscountCode { get; set; }
        public List<OrderDetailResponseDto>? OrderDetails { get; set; }

    }

    public class OrderDetailResponseDto
    {
        public int Id { get; set; }
        public int ColorId { get; set; }
        public string? ProductName { get; set; }
        public string? ColorName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}