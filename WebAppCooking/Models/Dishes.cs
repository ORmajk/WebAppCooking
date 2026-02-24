using System.ComponentModel.DataAnnotations;

namespace WebAppCooking.Models
{
    public class Dishes
    {
        [Key]
        public int IdDishe { get; set; }
        [Required]
        [StringLength(50)]
        public string? DisheName { get; set; }
        [Required]
        [StringLength(100)]
        public string? DisheDescription { get; set; }
        [Required]
        public int? DisheType { get; set; }
    }
}
