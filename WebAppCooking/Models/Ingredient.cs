using System.ComponentModel.DataAnnotations;

namespace WebAppCooking.Models
{
    public class Ingredient
    {
        [Key]
        public int IdIngredient { get; set; }
        [Required]
        [StringLength(50)]
        public string? IngredientName { get; set; }
        [Required]
        [StringLength(100)]
        public string? IngredientDescription { get; set; }
        [Required]
        public int Calories { get; set; }
        [Required]
        public int IdCategoryIngredient { get; set; }
    }
}
