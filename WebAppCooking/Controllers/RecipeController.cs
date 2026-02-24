using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppCooking.Data;

namespace WebAppCooking.Controllers
{
    public class RecipeController : Controller
    {
        private readonly UserAppContext _context;

        public RecipeController(UserAppContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.Users.Include(u => u.Role).ToList();
            return View(users);
        }
    }
}
