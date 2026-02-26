using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace WebAppCooking.Models
{
    public class RecipeIngredient
    {
        [Key]
        public int IdRecipeIngredients { get; set; }

        [Required]
        public int IdIngredient { get; set; }
        [ForeignKey("IdIngredient")]
        public virtual Ingredient? Ingredient { get; set; } // Было Ingredients, стало Ingredient

        [Required]
        public int IdRecipe { get; set; }
        [ForeignKey("IdRecipe")]
        public virtual Recipe? Recipe { get; set; } // Было Recipes, стало Recipe

        [Required]
        [StringLength(50)]
        public string? Note { get; set; }
    }
}
