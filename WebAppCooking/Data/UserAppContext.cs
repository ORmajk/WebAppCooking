using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebAppCooking.Models;

namespace WebAppCooking.Data
{
    public class UserAppContext : DbContext
    {
        public UserAppContext(DbContextOptions<UserAppContext> options): base(options) { } 
        
        public DbSet<User> users { get; set; }
    }
}
