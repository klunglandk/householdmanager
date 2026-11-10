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

        ShoppingItem? UpdateShoppingItem(int shoppingItemId, UpdateShoppingItemModel model);

        ShoppingItem? MarkAsBought(int shoppingItemId);

        ShoppingItem? ClearBoughtDate(int shoppingItemId);

        bool DeleteShoppingItem(int shoppingItemId);
    }

    public class ShoppingItemService : IShoppingItemService
    {
        public ShoppingItemService(HouseholdContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        private readonly HouseholdContext _context;
        private readonly IProductService _productService;

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

            var result = _context.ShoppingItems.Add(new ShoppingItem
            {
                Product = product,
                Amount = model.Amount,
                Unit = model.Unit
            });
            _context.SaveChanges();
            return result.Entity;
        }

        public ShoppingItem? UpdateShoppingItem(int shoppingItemId, UpdateShoppingItemModel model)
        {
            if (GetShoppingItem(shoppingItemId) is not { } item) return null;
            if (model.Amount is { } amount) item.Amount = amount;
            if (model.Unit is { } unit) item.Unit = unit;
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
}
