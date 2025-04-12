using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class QuestionCreateDto
    {
        public required int ProductId { get; set; }

        [StringLength(500)]
        public required string Content { get; set; }
    }

    public class QuestionResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Content { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
    }
}