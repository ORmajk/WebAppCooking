using System.ComponentModel.DataAnnotations;

namespace WebAppCooking.Models
{
    public class Role
    {
        [Key]
        public int IdRole { get; set; }
        [Required]
        public string? RoleName { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}
