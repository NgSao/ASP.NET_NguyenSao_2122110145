using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class ProductCreateDto
    {
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public required string Description { get; set; }
        public required decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public required IFormFile File { get; set; }
        public required int CategoryId { get; set; }
        public required int BrandId { get; set; }

        public required int Stock { get; set; }
        public required int Sold { get; set; }


    }

    public class ProductUpdateDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public IFormFile? File { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }

    }

    public class ProductResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int BrandId { get; set; }
        public string? BrandName { get; set; }
    }
}