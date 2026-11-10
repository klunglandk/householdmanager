using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IRecipeService
    {
        Recipe? GetRecipe(int recipeId);

        List<Recipe> GetAllRecipes(int householdId);

        Recipe CreateRecipe(CreateRecipeModel model);

        Recipe? UpdateRecipe(int recipeId, UpdateRecipeModel model);

        bool DeleteRecipe(int recipeId);

        Task<Recipe?> SetImageAsync(int recipeId, Stream content, CancellationToken cancellationToken);

        Stream? OpenImage(Recipe recipe);

        Recipe? RemoveImage(int recipeId);
    }

    public class RecipeService : IRecipeService
    {
        public RecipeService(HouseholdContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        private readonly HouseholdContext _context;
        private readonly IFileStorage _fileStorage;

        public Recipe? GetRecipe(int recipeId)
        {
            return _context.Recipes
                .Include(recipe => recipe.Ingredients)
                .ThenInclude(ingredient => ingredient.Product)
                .FirstOrDefault(recipe => recipe.Id == recipeId);
        }

        public List<Recipe> GetAllRecipes(int householdId)
        {
            return _context.Recipes
                .Include(recipe => recipe.Ingredients)
                .ThenInclude(ingredient => ingredient.Product)
                .Where(recipe => recipe.HouseholdId == householdId)
                .ToList();
        }

        public Recipe CreateRecipe(CreateRecipeModel model)
        {
            var result = _context.Recipes.Add(new Recipe
            {
                Name = model.Name,
                Instructions = model.Instructions,
                Servings = model.Servings,
                HouseholdId = model.HouseholdId
            });
            _context.SaveChanges();
            return result.Entity;
        }

        public Recipe? UpdateRecipe(int recipeId, UpdateRecipeModel model)
        {
            if (GetRecipe(recipeId) is not { } recipe) return null;
            if (model.Name is { } name) recipe.Name = name;
            if (model.Instructions is { } instructions) recipe.Instructions = instructions;
            if (model.Servings is { } servings) recipe.Servings = servings;
            _context.SaveChanges();
            return recipe;
        }

        public bool DeleteRecipe(int recipeId)
        {
            if (GetRecipe(recipeId) is not { } recipe) return false;
            _context.Recipes.Remove(recipe);
            _context.SaveChanges();
            if (recipe.ImagePath is { } imagePath) _fileStorage.Delete(imagePath);
            return true;
        }

        public async Task<Recipe?> SetImageAsync(int recipeId, Stream content, CancellationToken cancellationToken)
        {
            if (GetRecipe(recipeId) is not { } recipe) return null;

            var newImagePath = await _fileStorage.SaveAsync(content, AllowedFileKind.Image, cancellationToken);
            var oldImagePath = recipe.ImagePath;
            recipe.ImagePath = newImagePath;

            try
            {
                _context.SaveChanges();
            }
            catch
            {
                _fileStorage.Delete(newImagePath);
                throw;
            }

            if (oldImagePath is not null) _fileStorage.Delete(oldImagePath);
            return recipe;
        }

        public Stream? OpenImage(Recipe recipe)
        {
            return recipe.ImagePath is { } imagePath ? _fileStorage.Open(imagePath) : null;
        }

        public Recipe? RemoveImage(int recipeId)
        {
            if (GetRecipe(recipeId) is not { } recipe) return null;

            var oldImagePath = recipe.ImagePath;
            recipe.ImagePath = null;
            _context.SaveChanges();
            if (oldImagePath is not null) _fileStorage.Delete(oldImagePath);
            return recipe;
        }
    }

    public class CreateRecipeModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public string Instructions { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int? Servings { get; set; }

        public required int HouseholdId { get; set; }
    }

    public class UpdateRecipeModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }

        public string? Instructions { get; set; }

        [Range(1, int.MaxValue)]
        public int? Servings { get; set; }
    }
}
