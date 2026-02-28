using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebAppCooking.Data;
using WebAppCooking.Models;

namespace YourProjectName.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserAppContext _context;

        public UsersController(UserAppContext context)
        {
            _context = context;
        }

        // GET: Users
        public IActionResult Index()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .ToList();
            return View(users);
        }

        // GET: Users/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(m => m.IdUser == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/CreateUser
        public IActionResult CreateUser()
        {
            ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName");
            return View();
        }

        // POST: Users/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(User user)
        {
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                var existingUser = _context.Users
                    .FirstOrDefault(u => u.Login.ToLower() == user.Login.ToLower());

                if (existingUser != null)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                    ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
                    return View(user);
                }

                if (user.Age < 18 || user.Age > 120)
                {
                    ModelState.AddModelError("Age", "Возраст должен быть от 18 до 120 лет");
                    ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
                    return View(user);
                }

                if (!string.IsNullOrEmpty(user.Password))
                {
                    if (user.Password.Length < 6)
                    {
                        ModelState.AddModelError("Password", "Пароль должен содержать не менее 6 символов");
                        ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
                        return View(user);
                    }

                    bool hasLetter = user.Password.Any(char.IsLetter);
                    bool hasDigit = user.Password.Any(char.IsDigit);

                    if (!hasLetter || !hasDigit)
                    {
                        ModelState.AddModelError("Password", "Пароль должен содержать хотя бы одну букву и одну цифру");
                        ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
                        return View(user);
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Пароль не может быть пустым");
                    ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
                    return View(user);
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Пользователь успешно добавлен";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
            return View(user);
        }

        // GET: Users/EditUser/5
        public IActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
            return View(user);
        }

        // POST: Users/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int id, User user, int IdRole)
        {
            if (id != user.IdUser)
            {
                return NotFound();
            }

            user.IdRole = IdRole;

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = _context.Users
                        .FirstOrDefault(u => u.Login.ToLower() == user.Login.ToLower() && u.IdUser != id);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                        ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
                        return View(user);
                    }

                    _context.Update(user);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Пользователь успешно обновлен";
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.IdUser))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
             return RedirectToAction(nameof(Index));

            }

            ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
            return View(user);
            
        }

        // GET: Users/DeleteUser/5
        public IActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(m => m.IdUser == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUserConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Пользователь успешно удален";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/ChangePasswordUser/5
        public IActionResult ChangePasswordUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel
            {
                IdUser = user.IdUser,
                Login = user.Login
            };

            return View(model);
        }

        // POST: Users/ChangePasswordUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePasswordUser(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(model.IdUser);
                if (user == null)
                {
                    return NotFound();
                }

                user.Password = model.NewPassword;
                _context.Update(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Пароль успешно изменен";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.IdUser == id);
        }
    }

    public class ChangePasswordViewModel
    {
        public int IdUser { get; set; }
        public string? Login { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Пароль должен быть от 4 до 50 символов")]
        [Display(Name = "Новый пароль")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string? ConfirmPassword { get; set; }
    }
}