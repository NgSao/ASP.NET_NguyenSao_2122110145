using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{


    public class OrderCreateDto
    {

        public int UserId { get; set; }

        public int AddressId { get; set; }

        public string? PaymentMethod { get; set; }

        public string? Status { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal FinalAmount { get; set; }
        public List<OrderDetailCreateDto> OrderDetails { get; set; } = new();

    }


    public class OrderUpdateDto
    {
        public string? Status { get; set; }


    }




    public class OrderResponseDto : AuditableDtos
    {

        public int id { get; set; }
        public int UserId { get; set; }

        public int AddressId { get; set; }

        public string? PaymentMethodId { get; set; }

        public string? Status { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public UserDTO? User { get; set; }

        public AddressDTO? Address { get; set; }

        public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
    }


}
public class UserDTO
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
}

public class AddressDTO
{
    public int Id { get; set; }
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AddressDetail { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }
}

public class OrderDetailDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class OrderDetailCreateDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

}