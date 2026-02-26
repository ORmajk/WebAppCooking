using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // GET: RecipeAdmin
        public IActionResult Index()
        {
            // Проверяем, авторизован ли пользователь и имеет ли роль Admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) || userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var recipes = _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.Author)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredients)
                .ToList();

            return View(recipes);
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
                    .ThenInclude(ri => ri.Ingredients)
                .FirstOrDefault(m => m.IdRecipe == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: RecipeAdmin/Create
        public IActionResult Create()
        {
            ViewData["IdRecipeType"] = new SelectList(_context.RecipeTypes.ToList(), "IdRecipeType", "TypeName");
            ViewData["IdAuthor"] = new SelectList(_context.Authors.ToList(), "IdAuthor", "AuthorName");
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients.ToList(), "IdIngredient", "IngredientName");
            return View();
        }

        // POST: RecipeAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("IdRecipe,RecipeName,RecipeDescription,RecipeImage,RecipeDate,IdRecipeType,IdAuthor")] Recipe recipe, int[] selectedIngredients)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipe);
                _context.SaveChanges();

                // Добавляем ингредиенты
                if (selectedIngredients != null)
                {
                    foreach (var ingredientId in selectedIngredients)
                    {
                        _context.RecipeIngredients.Add(new RecipeIngredient
                        {
                            IdRecipe = recipe.IdRecipe,
                            IdIngredient = ingredientId
                        });
                    }
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdRecipeType"] = new SelectList(_context.RecipeTypes.ToList(), "IdRecipeType", "TypeName", recipe.IdRecipeType);
            ViewData["IdAuthor"] = new SelectList(_context.Authors.ToList(), "IdAuthor", "AuthorName", recipe.IdAuthor);
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients.ToList(), "IdIngredient", "IngredientName", selectedIngredients);

            return View(recipe);
        }

        // GET: RecipeAdmin/Edit/5
        public IActionResult Edit(int? id)
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

            ViewData["IdRecipeType"] = new SelectList(_context.RecipeTypes.ToList(), "IdRecipeType", "TypeName", recipe.IdRecipeType);
            ViewData["IdAuthor"] = new SelectList(_context.Authors.ToList(), "IdAuthor", "AuthorName", recipe.IdAuthor);

            var selectedIngredients = recipe.RecipeIngredients?.Select(ri => ri.IdIngredient).ToArray();
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients.ToList(), "IdIngredient", "IngredientName", selectedIngredients);

            return View(recipe);
        }

        // POST: RecipeAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("IdRecipe,RecipeName,RecipeDescription,RecipeImage,RecipeDate,IdRecipeType,IdAuthor")] Recipe recipe, int[] selectedIngredients)
        {
            if (id != recipe.IdRecipe)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    _context.SaveChanges();

                    // Обновляем ингредиенты
                    var existingIngredients = _context.RecipeIngredients.Where(ri => ri.IdRecipe == id).ToList();
                    _context.RecipeIngredients.RemoveRange(existingIngredients);

                    if (selectedIngredients != null)
                    {
                        foreach (var ingredientId in selectedIngredients)
                        {
                            _context.RecipeIngredients.Add(new RecipeIngredient
                            {
                                IdRecipe = recipe.IdRecipe,
                                IdIngredient = ingredientId
                            });
                        }
                    }
                    _context.SaveChanges();
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

            ViewData["IdRecipeType"] = new SelectList(_context.RecipeTypes.ToList(), "IdRecipeType", "TypeName", recipe.IdRecipeType);
            ViewData["IdAuthor"] = new SelectList(_context.Authors.ToList(), "IdAuthor", "AuthorName", recipe.IdAuthor);
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients.ToList(), "IdIngredient", "IngredientName", selectedIngredients);

            return View(recipe);
        }

        // GET: RecipeAdmin/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.Author)
                .FirstOrDefault(m => m.IdRecipe == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: RecipeAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var recipe = _context.Recipes
                .Include(r => r.RecipeIngredients)
                .FirstOrDefault(r => r.IdRecipe == id);

            if (recipe != null)
            {
                // Сначала удаляем связанные ингредиенты
                _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
                // Затем удаляем рецепт
                _context.Recipes.Remove(recipe);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.IdRecipe == id);
        }
    }
}