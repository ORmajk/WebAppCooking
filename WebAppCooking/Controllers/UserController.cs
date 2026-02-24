using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppCooking.Data;
using WebAppCooking.Models;
using System.Linq;

namespace YourProjectName.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserAppContext _context;

        public UsersController(UserAppContext context)
        {
            _context = context;
        }
        public  IActionResult Index()
        {
            var users =  _context.Users.Include(u => u.Role).ToList();
            return View(users);
        }
    }
}