using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class MediaRequestDto
    {
        public string? ImageUrl { get; set; }


    }

    public class MediaResponseDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }

        public int? ProductId { get; set; }

        public int? ColorId { get; set; }
    }


}