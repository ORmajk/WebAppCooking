using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebAppCooking.Data;
using WebAppCooking.Models;

namespace WebAppCooking.Controllers
{
    public class RecipeUserController : Controller
    {
        private readonly UserAppContext _context;

        public RecipeUserController(UserAppContext context)
        {
            _context = context;
        }

        // GET: RecipeUser
        public IActionResult Index(string searchString, string sortOrder, int? recipeTypeId, string dateFilter)
        {
            // Проверяем, авторизован ли пользователь
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Сохраняем параметры для представления
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentType"] = recipeTypeId;
            ViewData["CurrentDateFilter"] = dateFilter;

            // Параметры сортировки
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["TypeSortParam"] = sortOrder == "type" ? "type_desc" : "type";

            // Получаем все рецепты
            var recipes = _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.Author)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredients)
                .AsQueryable();

            // Фильтрация по типу рецепта
            if (recipeTypeId.HasValue && recipeTypeId.Value > 0)
            {
                recipes = recipes.Where(r => r.IdRecipeType == recipeTypeId);
            }

            // Фильтрация по дате
            if (!string.IsNullOrEmpty(dateFilter))
            {
                var today = DateTime.Today;
                switch (dateFilter)
                {
                    case "today":
                        recipes = recipes.Where(r => r.RecipeDate.Value.Date == today);
                        break;
                    case "week":
                        var weekStart = today.AddDays(-(int)today.DayOfWeek);
                        var weekEnd = weekStart.AddDays(7);
                        recipes = recipes.Where(r => r.RecipeDate >= weekStart && r.RecipeDate < weekEnd);
                        break;
                    case "month":
                        recipes = recipes.Where(r => r.RecipeDate.Value.Month == today.Month && r.RecipeDate.Value.Year == today.Year);
                        break;
                    case "year":
                        recipes = recipes.Where(r => r.RecipeDate.Value.Year == today.Year);
                        break;
                }
            }

            // Поиск по названию и описанию
            if (!string.IsNullOrEmpty(searchString))
            {
                recipes = recipes.Where(r => r.RecipeName.Contains(searchString) ||
                                            r.RecipeDescription.Contains(searchString));
            }

            // Сортировка
            switch (sortOrder)
            {
                case "name_desc":
                    recipes = recipes.OrderByDescending(r => r.RecipeName);
                    break;
                case "date":
                    recipes = recipes.OrderBy(r => r.RecipeDate);
                    break;
                case "date_desc":
                    recipes = recipes.OrderByDescending(r => r.RecipeDate);
                    break;
                case "type":
                    recipes = recipes.OrderBy(r => r.RecipeType.RecipeTypeName);
                    break;
                case "type_desc":
                    recipes = recipes.OrderByDescending(r => r.RecipeType.RecipeTypeName);
                    break;
                default:
                    recipes = recipes.OrderBy(r => r.RecipeName);
                    break;
            }

            // Получаем список типов для фильтра
            ViewBag.RecipeTypes = _context.RecipeTypes.ToList();

            return View(recipes.ToList());
        }
        // GET: RecipeUser/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.Author)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredients)
                .FirstOrDefault(m => m.IdRecipe == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }
    }
}