using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppCooking.Data;
using WebAppCooking.Models;

public class RecipeAdminController : Controller
{
    private readonly UserAppContext _context;

    public RecipeAdminController(UserAppContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        try
        {
            // Проверка авторизации
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Проверка роли
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("IndexMenu", "Home");
            }

            // Получение данных с включением связанных сущностей
            var recipes = _context.Recipes
                .Include(r => r.RecipeType)
                .Include(r => r.Author)
                .ToList();

            // Важно: даже если нет рецептов, возвращаем пустой список, не null
            return View(recipes ?? new List<Recipe>());
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Ошибка в RecipeAdmin.Index: {ex.Message}");
            return View(new List<Recipe>()); // Возвращаем пустой список при ошибке
        }
    }
    // GET: RecipeAdmin/Create
    public IActionResult Create()
    {
        ViewData["RecipeTypeId"] = new SelectList(_context.RecipeTypes, "IdRecipeType", "RecipeTypeName");
        ViewData["AuthorId"] = new SelectList(_context.Authors, "IdAuthor", "AuthorName");
        ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "IdIngredient", "IngredientName");
        return View();
    }

    // POST: RecipeAdmin/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Recipe recipe, int[] selectedIngredients)
    {
        // Удаляем навигационные свойства из ModelState
        ModelState.Remove("RecipeType");
        ModelState.Remove("Author");
        ModelState.Remove("RecipeIngredients");

        if (ModelState.IsValid)
        {
            _context.Add(recipe);
            _context.SaveChanges();

            // Добавляем ингредиенты
            if (selectedIngredients != null && selectedIngredients.Any())
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

            TempData["SuccessMessage"] = "Рецепт успешно добавлен";
            return RedirectToAction(nameof(Index));
        }

        ViewData["RecipeTypeId"] = new SelectList(_context.RecipeTypes, "IdRecipeType", "RecipeTypeName", recipe.IdRecipeType);
        ViewData["AuthorId"] = new SelectList(_context.Authors, "IdAuthor", "AuthorName", recipe.IdAuthor);
        ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "IdIngredient", "IngredientName", selectedIngredients);
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

        ViewData["RecipeTypeId"] = new SelectList(_context.RecipeTypes, "IdRecipeType", "RecipeTypeName", recipe.IdRecipeType);
        ViewData["AuthorId"] = new SelectList(_context.Authors, "IdAuthor", "AuthorName", recipe.IdAuthor);

        var selectedIngredients = recipe.RecipeIngredients?.Select(ri => ri.IdIngredient).ToArray();
        ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "IdIngredient", "IngredientName", selectedIngredients);

        return View(recipe);
    }

    // POST: RecipeAdmin/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Recipe recipe, int[] selectedIngredients)
    {
        if (id != recipe.IdRecipe)
        {
            return NotFound();
        }

        // Удаляем навигационные свойства из ModelState
        ModelState.Remove("RecipeType");
        ModelState.Remove("Author");
        ModelState.Remove("RecipeIngredients");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(recipe);
                _context.SaveChanges();

                // Обновляем ингредиенты
                var existingIngredients = _context.RecipeIngredients.Where(ri => ri.IdRecipe == id).ToList();
                _context.RecipeIngredients.RemoveRange(existingIngredients);

                if (selectedIngredients != null && selectedIngredients.Any())
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

        ViewData["RecipeTypeId"] = new SelectList(_context.RecipeTypes, "IdRecipeType", "RecipeTypeName", recipe.IdRecipeType);
        ViewData["AuthorId"] = new SelectList(_context.Authors, "IdAuthor", "AuthorName", recipe.IdAuthor);
        ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "IdIngredient", "IngredientName", selectedIngredients);
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