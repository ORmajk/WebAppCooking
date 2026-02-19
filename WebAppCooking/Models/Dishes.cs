using System.ComponentModel.DataAnnotations;

namespace WebAppCooking.Models
{
    public class Dishes
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string? DisheName { get; set; }

    }
}
