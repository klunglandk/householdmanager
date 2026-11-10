using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
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

        Ingredient? AddIngredient(int recipeId, CreateIngredientModel model);

        Ingredient? UpdateIngredient(int ingredientId, UpdateIngredientModel model);

        bool DeleteIngredient(int ingredientId);
    }

    public class RecipeService : IRecipeService
    {
        public RecipeService(HouseholdContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        private readonly HouseholdContext _context;
        private readonly IProductService _productService;

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
                ImagePath = model.ImagePath,
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
            if (model.ImagePath is { } imagePath) recipe.ImagePath = imagePath;
            _context.SaveChanges();
            return recipe;
        }

        public bool DeleteRecipe(int recipeId)
        {
            if (GetRecipe(recipeId) is not { } recipe) return false;
            _context.Recipes.Remove(recipe);
            _context.SaveChanges();
            return true;
        }

        public Ingredient? AddIngredient(int recipeId, CreateIngredientModel model)
        {
            var recipe = _context.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe is null) return null;
            if (_productService.GetProduct(model.ProductId) is not { } product) return null;

            var result = _context.Ingredients.Add(new Ingredient
            {
                Recipe = recipe,
                Product = product,
                Amount = model.Amount,
                Unit = model.Unit
            });
            _context.SaveChanges();
            return result.Entity;
        }

        public Ingredient? UpdateIngredient(int ingredientId, UpdateIngredientModel model)
        {
            var ingredient = _context.Ingredients
                .Include(i => i.Product)
                .FirstOrDefault(i => i.Id == ingredientId);
            if (ingredient is null) return null;

            if (model.Amount is { } amount) ingredient.Amount = amount;
            if (model.Unit is { } unit) ingredient.Unit = unit;
            _context.SaveChanges();
            return ingredient;
        }

        public bool DeleteIngredient(int ingredientId)
        {
            var ingredient = _context.Ingredients.FirstOrDefault(i => i.Id == ingredientId);
            if (ingredient is null) return false;
            _context.Ingredients.Remove(ingredient);
            _context.SaveChanges();
            return true;
        }
    }

    public class CreateRecipeModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }

        public string Instructions { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int? Servings { get; set; }

        public string? ImagePath { get; set; }

        public required int HouseholdId { get; set; }
    }

    public class UpdateRecipeModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }

        public string? Instructions { get; set; }

        [Range(1, int.MaxValue)]
        public int? Servings { get; set; }

        public string? ImagePath { get; set; }
    }

    public class CreateIngredientModel
    {
        public required int ProductId { get; set; }

        [Range(typeof(decimal), "0.0001", "1000000")]
        public required decimal Amount { get; set; }

        public required MeasurementUnit Unit { get; set; }
    }

    public class UpdateIngredientModel
    {
        [Range(typeof(decimal), "0.0001", "1000000")]
        public decimal? Amount { get; set; }

        public MeasurementUnit? Unit { get; set; }
    }
}
