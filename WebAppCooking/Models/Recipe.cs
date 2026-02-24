using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppCooking.Models
{
    public class Recipe
    {
        [Key]
        public int IdRecipe { get; set; }
        [Required]
        [StringLength(50)]
        public string? RecipeName { get; set; }
        [Required]
        [StringLength(200)]
        public string? RecipeDescription { get; set; }
        [Required]
        public string? RecipeImage { get; set; }
        [Required]
        public DateTime? RecipeDate { get; set; }
        [Required]
        public int? IdRecipeType { get; set; }
        [ForeignKey("IdRecipeType")]
        public virtual RecipeType? RecipeType { get; set; }
        public int? IdAuthor { get; set; }
        [ForeignKey("IdAuthor")]
        public virtual Author? Author { get; set; }
        public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }

    }
}
