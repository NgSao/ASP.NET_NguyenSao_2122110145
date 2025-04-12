using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class ProductCreateDto
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        public required int CategoryId { get; set; }
        public required int BrandId { get; set; }

    }

    public class ProductUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }

    }

    public class ProductResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }

    }


    public class ProductResponseColorDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public IEnumerable<ProductVariantResponseColorDto>? Variants { get; set; }
        public IEnumerable<ImageResponseColorDto>? Images { get; set; }
    }


    public class ProductVariantResponseColorDto
    {
        public int Id { get; set; }
        public string? Storage { get; set; }
        public decimal BasePrice { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public IEnumerable<ProductColorResponseColorDto>? ProductColors { get; set; }
    }

    public class ProductColorResponseColorDto
    {
        public int Id { get; set; }
        public string? ColorName { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }

    }
    public class ImageResponseColorDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
    }

}

