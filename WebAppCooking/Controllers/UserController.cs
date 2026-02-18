using Microsoft.AspNetCore.Mvc;
using WebAppCooking.Data;

namespace WebAppCooking.Controllers
{
    public class UserController : Controller
    {
        private readonly UserAppContext _context;
        public UserController(UserAppContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.users.ToList();
            return View(users);
        }
    }
}
