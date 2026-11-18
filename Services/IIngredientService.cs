using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;

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
}
