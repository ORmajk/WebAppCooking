using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Users/Create
        public IActionResult Create()
        {
            // Получаем список ролей для выпадающего списка
            ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            // Убираем проверку роли из ModelState, чтобы избежать ошибок валидации
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                // Проверяем, существует ли пользователь с таким логином
                var existingUser = _context.Users
                    .FirstOrDefault(u => u.Login.ToLower() == user.Login.ToLower());

                if (existingUser != null)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                    ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
                    return View(user);
                }

                // Добавляем пользователя
                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Пользователь успешно добавлен";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(_context.Roles, "IdRole", "RoleName", user.IdRole);
            return View(user);
        }

        // GET: Users/Edit/5
        public IActionResult Edit(int? id)
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

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (id != user.IdUser)
            {
                return NotFound();
            }

            // Убираем проверку роли из ModelState
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                try
                {
                    // Проверяем уникальность логина (исключая текущего пользователя)
                    var existingUser = _context.Users
                        .FirstOrDefault(u => u.Login.ToLower() == user.Login.ToLower()
                                            && u.IdUser != id);

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

        // GET: Users/Delete/5
        public IActionResult Delete(int? id)
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
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

        // GET: Users/ChangePassword/5
        public IActionResult ChangePassword(int? id)
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

        // POST: Users/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(model.IdUser);
                if (user == null)
                {
                    return NotFound();
                }

                // Обновляем пароль
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

    // ViewModel для смены пароля
    public class ChangePasswordViewModel
    {
        public int IdUser { get; set; }
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Пароль должен быть от 4 до 50 символов")]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }
    }
}