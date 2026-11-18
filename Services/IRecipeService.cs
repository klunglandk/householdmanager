using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;

namespace HouseholdManager.Services
{
    public interface IRecipeService
    {
        Recipe? GetRecipe(int recipeId);

        List<Recipe> GetAllRecipes();

        Recipe CreateRecipe(CreateRecipeModel model);

        Recipe? UpdateRecipe(int recipeId, UpdateRecipeModel model);

        bool DeleteRecipe(int recipeId);

        Task<Recipe?> SetImageAsync(int recipeId, Stream content, CancellationToken cancellationToken);

        Stream? OpenImage(Recipe recipe);

        Recipe? RemoveImage(int recipeId);
    }
}
