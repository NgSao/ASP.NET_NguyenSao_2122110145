using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("brand")]
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? ImageUrl { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

    }
}
