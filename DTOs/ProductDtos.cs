using System.ComponentModel.DataAnnotations;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.DTOs
{
    public class ProductCreateDto
    {
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public required int CategoryId { get; set; }
        public required int BrandId { get; set; }
        public List<MediaRequestDto>? Medias { get; set; } // Hoặc kiểu khác nếu cần thiết
    }

    public class ProductUpdateDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public List<MediaRequestDto> Images { get; set; } = new List<MediaRequestDto>();
    }



    public class ProductResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public List<VariantResponseDto>? Variants { get; set; }
        public List<MediaResponseDto>? Media { get; set; }
    }


    //Từ tính sau
    public class ProductDataResponse : AuditableDtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public CategoryDataResponseDto? Category { get; set; }
        public BrandDataResponseDto? Brand { get; set; }
        public List<VariantDataResponseDto>? Variants { get; set; }
        public List<MediaDataResponseDto>? Media { get; set; }
    }

    public class CategoryDataResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public List<CategoryDataResponseDto>? Children { get; set; }
    }
    public class BrandDataResponseDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class VariantDataResponseDto
    {
        public int Id { get; set; }
        public string? Storage { get; set; }
        public decimal BasePrice { get; set; }
        List<ColorDataResponseDto>? Colors { get; set; }
    }


    public class ColorDataResponseDto
    {
        public int Id { get; set; }
        public string? ColorName { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public MediaDataResponseDto? Media { get; set; }

    }
    public class MediaDataResponseDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
    }

}

