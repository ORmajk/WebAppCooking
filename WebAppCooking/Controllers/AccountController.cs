using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppCooking.Data;
using WebAppCooking.Models;

public class AccountController : Controller
{
    private readonly UserAppContext _context;

    public AccountController(UserAppContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);

            if (user != null)
            {
                // Простая аутентификация через сессию
                HttpContext.Session.SetString("UserId", user.IdUser.ToString());
                HttpContext.Session.SetString("UserLogin", user.Login);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role?.RoleName ?? "");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неверный логин или пароль");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Проверяем, существует ли уже пользователь с таким логином
            var existingUser = _context.Users.FirstOrDefault(u => u.Login == model.Login);
            if (existingUser != null)
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                return View(model);
            }

            // Создаем нового пользователя
            var user = new User
            {
                Login = model.Login,
                Password = model.Password,
                Name = model.Name,
                Age = model.Age,
                IdRole = 2 
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Автоматически логиним после регистрации
            HttpContext.Session.SetString("UserId", user.IdUser.ToString());
            HttpContext.Session.SetString("UserLogin", user.Login);
            HttpContext.Session.SetString("UserName", user.Name);

            return RedirectToAction("Index", "Home");
        }
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}