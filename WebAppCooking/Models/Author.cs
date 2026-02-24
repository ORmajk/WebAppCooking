using System.ComponentModel.DataAnnotations;

namespace WebAppCooking.Models
{
    public class Author
    {
        [Key]
        public int IdAuthor { get; set; }
        [Required]
        [StringLength(50)]
        public string? AuthorName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        [StringLength(50)]
        public string? AuthorSpecialization { get; set; }
        [Required]
        [StringLength(50)]
        public string? AuthorCountry { get; set; }
        public ICollection<Recipe>? Recipes { get; set; }

    }
}
