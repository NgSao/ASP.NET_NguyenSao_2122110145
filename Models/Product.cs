using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenSao_2122110145.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }


    }
}
