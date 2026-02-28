using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebAppCooking.Data;
using WebAppCooking.Models;

namespace WebAppCooking.Controllers
{
    public class RecipeAdminController : Controller
    {
        private readonly UserAppContext _context;

        public RecipeAdminController(UserAppContext context)
        {
            _context = context;
        }

        // GET: RecipeAdmin с фильтрацией, поиском и сортировкой
        public IActionResult Index(string searchString, string sortOrder, int? recipeTypeId, string dateFilter)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) || userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentType"] = recipeTypeId;
            ViewData["CurrentDateFilter"] = dateFilter;

            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["TypeSortParam"] = sortOrder == "type" ? "type_desc" : "type";

            var recipes = _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.Author)
                .Include(r => r.RecipeIngredients)
                .AsQueryable();

            if (recipeTypeId.HasValue && recipeTypeId.Value > 0)
            {
                recipes = recipes.Where(r => r.IdRecipeType == recipeTypeId);
            }

            if (!string.IsNullOrEmpty(dateFilter))
            {
                var today = DateTime.Today;
                switch (dateFilter)
                {
                    case "today":
                        recipes = recipes.Where(r => r.RecipeDate.HasValue &&
                                                     r.RecipeDate.Value.Date == today);
                        break;
                    case "week":
                        var weekStart = today.AddDays(-(int)today.DayOfWeek);
                        var weekEnd = weekStart.AddDays(7);
                        recipes = recipes.Where(r => r.RecipeDate.HasValue &&
                                                     r.RecipeDate >= weekStart &&
                                                     r.RecipeDate < weekEnd);
                        break;
                    case "month":
                        recipes = recipes.Where(r => r.RecipeDate.HasValue &&
                                                     r.RecipeDate.Value.Month == today.Month &&
                                                     r.RecipeDate.Value.Year == today.Year);
                        break;
                    case "year":
                        recipes = recipes.Where(r => r.RecipeDate.HasValue &&
                                                     r.RecipeDate.Value.Year == today.Year);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.ToLower();
                recipes = recipes.Where(r => r.RecipeName.ToLower().Contains(searchLower) ||
                                            (r.RecipeDescription != null &&
                                             r.RecipeDescription.ToLower().Contains(searchLower)));
            }

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

            ViewBag.RecipeTypes = _context.RecipeTypes.ToList();

            return View(recipes.ToList());
        }

        // GET: RecipeAdmin/Details/5
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
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefault(m => m.IdRecipe == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: RecipeAdmin/CreateRecipe
        public IActionResult CreateRecipe()
        {
            ViewData["IdRecipeType"] = new SelectList(_context.RecipeTypes, "IdRecipeType", "RecipeTypeName");
            ViewData["IdAuthor"] = new SelectList(_context.Authors, "IdAuthor", "AuthorName");
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "IdIngredient", "IngredientName");
            return View();
        }

        // POST: RecipeAdmin/CreateRecipe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRecipe(Recipe recipe, int[] selectedIngredients)
        {
            try
            {
                ModelState.Remove("RecipeType");
                ModelState.Remove("Author");
                ModelState.Remove("RecipeIngredients");

                if (recipe.RecipeName?.Length > 50)
                {
                    ModelState.AddModelError("RecipeName", "Название не может быть длиннее 50 символов");
                    return View(recipe);
                }

                if (recipe.RecipeDescription?.Length > 200)
                {
                    ModelState.AddModelError("RecipeDescription", "Описание не может быть длиннее 200 символов");
                    return View(recipe);
                }

                if (string.IsNullOrEmpty(recipe.RecipeName))
                {
                    ModelState.AddModelError("RecipeName", "Название обязательно");
                }

                if (string.IsNullOrEmpty(recipe.RecipeDescription))
                {
                    ModelState.AddModelError("RecipeDescription", "Описание обязательно");
                }

                if (recipe.RecipeDate == null)
                {
                    ModelState.AddModelError("RecipeDate", "Дата обязательна");
                }

                if (recipe.IdRecipeType.HasValue)
                {
                    var recipeTypeExists = _context.RecipeTypes.Any(rt => rt.IdRecipeType == recipe.IdRecipeType);
                    if (!recipeTypeExists)
                    {
                        ModelState.AddModelError("IdRecipeType", "Выбранный тип не существует");
                    }
                }

                if (recipe.IdAuthor.HasValue)
                {
                    var authorExists = _context.Authors.Any(a => a.IdAuthor == recipe.IdAuthor);
                    if (!authorExists)
                    {
                        ModelState.AddModelError("IdAuthor", "Выбранный автор не существует");
                    }
                }

                var existingRecipe = _context.Recipes
                    .FirstOrDefault(r => r.RecipeName == recipe.RecipeName);
                if (existingRecipe != null)
                {
                    ModelState.AddModelError("RecipeName", "Рецепт с таким названием уже существует");
                }

                if (ModelState.IsValid)
                {
                    _context.Add(recipe);
                    _context.SaveChanges();

                    if (selectedIngredients != null && selectedIngredients.Any())
                    {
                        foreach (var ingredientId in selectedIngredients)
                        {
                            if (_context.Ingredients.Any(i => i.IdIngredient == ingredientId))
                            {
                                _context.RecipeIngredients.Add(new RecipeIngredient
                                {
                                    IdRecipe = recipe.IdRecipe,
                                    IdIngredient = ingredientId,
                                    Note = ""
                                });
                            }
                        }
                        _context.SaveChanges();
                    }

                    TempData["SuccessMessage"] = "Рецепт успешно добавлен";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "Неизвестная ошибка базы данных";
                ModelState.AddModelError("", $"Ошибка базы данных: {innerMessage}");

                Console.WriteLine($"Ошибка при создании рецепта: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Общая ошибка: {ex.Message}");
            }

            ViewData["IdRecipeType"] = new SelectList(_context.RecipeTypes, "IdRecipeType", "RecipeTypeName", recipe.IdRecipeType);
            ViewData["IdAuthor"] = new SelectList(_context.Authors, "IdAuthor", "AuthorName", recipe.IdAuthor);
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "IdIngredient", "IngredientName", selectedIngredients);

            return View(recipe);
        }

        // GET: RecipeAdmin/EditRecipe/5
        public IActionResult EditRecipe(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefault(r => r.IdRecipe == id);

            if (recipe == null)
            {
                return NotFound();
            }

            ViewData["IdRecipeType"] = new SelectList(_context.RecipeTypes, "IdRecipeType", "RecipeTypeName", recipe.IdRecipeType);
            ViewData["IdAuthor"] = new SelectList(_context.Authors, "IdAuthor", "AuthorName", recipe.IdAuthor);

            var selectedIngredients = recipe.RecipeIngredients?.Select(ri => ri.IdIngredient).ToArray();
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "IdIngredient", "IngredientName", selectedIngredients);

            return View(recipe);
        }

        // POST: RecipeAdmin/EditRecipe/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRecipe(int id, Recipe recipe, int[] selectedIngredients)
        {
            if (id != recipe.IdRecipe)
            {
                return NotFound();
            }

            ModelState.Remove("RecipeType");
            ModelState.Remove("Author");
            ModelState.Remove("RecipeIngredients");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    _context.SaveChanges();

                    var existingIngredients = _context.RecipeIngredients.Where(ri => ri.IdRecipe == id).ToList();
                    _context.RecipeIngredients.RemoveRange(existingIngredients);

                    if (selectedIngredients != null && selectedIngredients.Any())
                    {
                        foreach (var ingredientId in selectedIngredients)
                        {
                            _context.RecipeIngredients.Add(new RecipeIngredient
                            {
                                IdRecipe = recipe.IdRecipe,
                                IdIngredient = ingredientId,
                                Note = ""
                            });
                        }
                    }
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Рецепт успешно обновлен";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.IdRecipe))
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

            ViewData["IdRecipeType"] = new SelectList(_context.RecipeTypes, "IdRecipeType", "RecipeTypeName", recipe.IdRecipeType);
            ViewData["IdAuthor"] = new SelectList(_context.Authors, "IdAuthor", "AuthorName", recipe.IdAuthor);
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "IdIngredient", "IngredientName", selectedIngredients);
            return View(recipe);
        }

        // GET: RecipeAdmin/DeleteRecipe/5
        public IActionResult DeleteRecipe(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.Author)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefault(m => m.IdRecipe == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: RecipeAdmin/DeleteRecipe/5
        [HttpPost, ActionName("DeleteRecipe")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRecipeConfirmed(int id)
        {
            var recipe = _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefault(r => r.IdRecipe == id);

            if (recipe != null)
            {
                _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
                _context.Recipes.Remove(recipe);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Рецепт успешно удален";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.IdRecipe == id);
        }
    }
}