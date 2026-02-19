using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppCooking.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        [Required]
        [StringLength(50)]
        public string? Login { get; set; }
        [Required]
        [StringLength(50)]
        public string? Password { get; set; }
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
        [Range(18, 100)]
        public int Age { get; set; }
        public int IdRole {  get; set; }
        [ForeignKey("IdRole")]
        public Role? Role { get; set; }
    }
}
