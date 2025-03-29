using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Avatar { get; set; }

        public string? Email { get; set; }
    }
}
