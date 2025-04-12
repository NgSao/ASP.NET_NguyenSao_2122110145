using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class ReviewCreateDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        [Required]
        public int ProductId { get; set; }
    }

    public class ReviewResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}