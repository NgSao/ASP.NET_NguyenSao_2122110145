namespace NguyenSao_2122110145.DTOs
{
    public abstract class AuditableDtos
    {
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
