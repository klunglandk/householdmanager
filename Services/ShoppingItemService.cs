using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IShoppingItemService
    {
        ShoppingItem? GetShoppingItem(int shoppingItemId);

        List<ShoppingItem> GetAllShoppingItems(int householdId);

        ShoppingItem? CreateShoppingItem(CreateShoppingItemModel model);

        List<ShoppingItem>? AddFromRecipe(int recipeId, AddRecipeToShoppingListModel model);

        ShoppingItem? UpdateShoppingItem(int shoppingItemId, UpdateShoppingItemModel model);

        ShoppingItem? MarkAsBought(int shoppingItemId);

        ShoppingItem? ClearBoughtDate(int shoppingItemId);

        bool DeleteShoppingItem(int shoppingItemId);
    }

    public class ShoppingItemService : IShoppingItemService
    {
        public ShoppingItemService(HouseholdContext context, IProductService productService, IRecipeService recipeService)
        {
            _context = context;
            _productService = productService;
            _recipeService = recipeService;
        }

        private readonly HouseholdContext _context;
        private readonly IProductService _productService;
        private readonly IRecipeService _recipeService;

        public ShoppingItem? GetShoppingItem(int shoppingItemId)
        {
            return _context.ShoppingItems
                .Include(item => item.Product)
                .FirstOrDefault(item => item.Id == shoppingItemId);
        }

        public List<ShoppingItem> GetAllShoppingItems(int householdId)
        {
            return _context.ShoppingItems
                .Include(item => item.Product)
                .Where(item => item.Product.HouseholdId == householdId)
                .ToList();
        }

        public ShoppingItem? CreateShoppingItem(CreateShoppingItemModel model)
        {
            if (_productService.GetProduct(model.ProductId) is not { } product) return null;

            var unbought = GetUnboughtItems([product.Id]);
            var result = AddOrMerge(unbought, product, model.Amount, model.Unit);
            _context.SaveChanges();
            return result;
        }

        public List<ShoppingItem>? AddFromRecipe(int recipeId, AddRecipeToShoppingListModel model)
        {
            if (_recipeService.GetRecipe(recipeId) is not { } recipe) return null;

            var ingredients = recipe.Ingredients;
            if (model.IngredientIds is { Count: > 0 } ingredientIds)
            {
                ingredients = recipe.Ingredients.Where(ingredient => ingredientIds.Contains(ingredient.Id)).ToList();
                if (ingredients.Count != ingredientIds.Distinct().Count()) return null;
            }

            var scale = 1m;
            if (model.Servings is { } wantedServings && recipe.Servings is { } recipeServings && recipeServings > 0)
            {
                scale = (decimal)wantedServings / recipeServings;
            }

            var unbought = GetUnboughtItems(ingredients.Select(ingredient => ingredient.ProductId).Distinct().ToList());

            var result = new List<ShoppingItem>();
            foreach (var ingredient in ingredients)
            {
                var item = AddOrMerge(unbought, ingredient.Product, ingredient.Amount * scale, ingredient.Unit);
                if (!result.Contains(item)) result.Add(item);
            }

            _context.SaveChanges();
            return result;
        }

        public ShoppingItem? UpdateShoppingItem(int shoppingItemId, UpdateShoppingItemModel model)
        {
            if (GetShoppingItem(shoppingItemId) is not { } item) return null;
            if (model.Amount is { } amount) item.Amount = amount;
            if (model.Unit is { } unit) item.Unit = unit;

            var normalized = MeasurementConverter.Normalize(item.Amount, item.Unit);
            item.Amount = normalized.Amount;
            item.Unit = normalized.Unit;

            _context.SaveChanges();
            return item;
        }

        public ShoppingItem? MarkAsBought(int shoppingItemId)
        {
            if (GetShoppingItem(shoppingItemId) is not { } item) return null;
            item.BoughtDate = DateTimeOffset.UtcNow;
            _context.SaveChanges();
            return item;
        }

        public ShoppingItem? ClearBoughtDate(int shoppingItemId)
        {
            if (GetShoppingItem(shoppingItemId) is not { } item) return null;
            item.BoughtDate = null;
            _context.SaveChanges();
            return item;
        }

        public bool DeleteShoppingItem(int shoppingItemId)
        {
            if (GetShoppingItem(shoppingItemId) is not { } item) return false;
            _context.ShoppingItems.Remove(item);
            _context.SaveChanges();
            return true;
        }

        private List<ShoppingItem> GetUnboughtItems(List<int> productIds)
        {
            return _context.ShoppingItems
                .Include(item => item.Product)
                .Where(item => productIds.Contains(item.ProductId) && item.BoughtDate == null)
                .ToList();
        }

        private ShoppingItem AddOrMerge(List<ShoppingItem> unbought, Product product, decimal amount, MeasurementUnit unit)
        {
            var existing = unbought.FirstOrDefault(item => item.ProductId == product.Id && MeasurementConverter.CanCombine(item.Unit, unit));
            if (existing is not null)
            {
                var combined = MeasurementConverter.Add(existing.Amount, existing.Unit, amount, unit);
                existing.Amount = combined.Amount;
                existing.Unit = combined.Unit;
                return existing;
            }

            var normalized = MeasurementConverter.Normalize(amount, unit);
            var newItem = new ShoppingItem
            {
                ProductId = product.Id,
                Product = product,
                Amount = normalized.Amount,
                Unit = normalized.Unit
            };
            _context.ShoppingItems.Add(newItem);
            unbought.Add(newItem);
            return newItem;
        }
    }

    public class CreateShoppingItemModel
    {
        public required int ProductId { get; set; }

        [Range(typeof(decimal), "0.0001", "1000000")]
        public required decimal Amount { get; set; }

        public required MeasurementUnit Unit { get; set; }
    }

    public class UpdateShoppingItemModel
    {
        [Range(typeof(decimal), "0.0001", "1000000")]
        public decimal? Amount { get; set; }

        public MeasurementUnit? Unit { get; set; }
    }

    public class AddRecipeToShoppingListModel
    {
        public List<int>? IngredientIds { get; set; }

        [Range(1, int.MaxValue)]
        public int? Servings { get; set; }
    }
}
