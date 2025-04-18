
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("otp")]
    public class Otp
    {

        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Code { get; set; }
        public int Attempts { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}