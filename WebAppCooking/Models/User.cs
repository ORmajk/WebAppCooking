using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppCooking.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        [Required]
        public string? Login { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Name { get; set; }
        [Range(18, 100)]
        public int Age { get; set; }
        public int IdRole {  get; set; }
        [ForeignKey("IdRole")]
        public Role? Role { get; set; }
    }
}
