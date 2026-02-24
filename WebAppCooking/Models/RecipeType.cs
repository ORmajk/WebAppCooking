using System.ComponentModel.DataAnnotations;

namespace WebAppCooking.Models
{
    public class RecipeType
    {
        [Key]
        public int IdRecipeType { get; set; }
        [Required]
        [StringLength(50)]
        public string? RecipeTypeName { get; set; }
        public ICollection<Recipe>? Recipes { get; set; }

    }
}
