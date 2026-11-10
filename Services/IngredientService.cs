using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IIngredientService
    {
        Ingredient? GetIngredient(int ingredientId);

        List<Ingredient> GetAllIngredients(int recipeId);

        Ingredient? CreateIngredient(CreateIngredientModel model);

        Ingredient? UpdateIngredient(int ingredientId, UpdateIngredientModel model);

        bool DeleteIngredient(int ingredientId);
    }

    public class IngredientService : IIngredientService
    {
        public IngredientService(HouseholdContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        private readonly HouseholdContext _context;
        private readonly IProductService _productService;

        public Ingredient? GetIngredient(int ingredientId)
        {
            return _context.Ingredients
                .Include(ingredient => ingredient.Product)
                .FirstOrDefault(ingredient => ingredient.Id == ingredientId);
        }

        public List<Ingredient> GetAllIngredients(int recipeId)
        {
            return _context.Ingredients
                .Include(ingredient => ingredient.Product)
                .Where(ingredient => ingredient.RecipeId == recipeId)
                .ToList();
        }

        public Ingredient? CreateIngredient(CreateIngredientModel model)
        {
            var recipe = _context.Recipes.FirstOrDefault(r => r.Id == model.RecipeId);
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
            if (GetIngredient(ingredientId) is not { } ingredient) return null;

            if (model.Amount is { } amount) ingredient.Amount = amount;
            if (model.Unit is { } unit) ingredient.Unit = unit;
            _context.SaveChanges();
            return ingredient;
        }

        public bool DeleteIngredient(int ingredientId)
        {
            if (GetIngredient(ingredientId) is not { } ingredient) return false;
            _context.Ingredients.Remove(ingredient);
            _context.SaveChanges();
            return true;
        }
    }

    public class CreateIngredientModel
    {
        public required int RecipeId { get; set; }

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
