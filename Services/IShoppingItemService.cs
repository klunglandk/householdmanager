using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;

namespace HouseholdManager.Services
{
    public interface IShoppingItemService
    {
        ShoppingItem? GetShoppingItem(int shoppingItemId);

        List<ShoppingItem> GetAllShoppingItems();

        ShoppingItem? CreateShoppingItem(CreateShoppingItemModel model);

        List<ShoppingItem>? AddFromRecipe(int recipeId, AddRecipeToShoppingListModel model);

        ShoppingItem? UpdateShoppingItem(int shoppingItemId, UpdateShoppingItemModel model);

        ShoppingItem? MarkAsBought(int shoppingItemId);

        ShoppingItem? ClearBoughtDate(int shoppingItemId);

        bool DeleteShoppingItem(int shoppingItemId);

        int DeleteBoughtShoppingItems();
    }
}
