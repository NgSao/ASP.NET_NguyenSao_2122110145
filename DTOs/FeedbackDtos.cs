using System.ComponentModel.DataAnnotations;

namespace NguyenSao_2122110145.DTOs
{
    public class FeedbackCreateDto
    {
        public required int QuestionId { get; set; }

        [StringLength(1000)]
        public required string? Content { get; set; }
    }

    public class FeedbackResponseDto : AuditableDtos
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string? Content { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
    }
}