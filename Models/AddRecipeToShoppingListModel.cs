using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models
{
    public class AddRecipeToShoppingListModel
    {
        public List<int>? IngredientIds { get; set; }

        [Range(1, int.MaxValue)]
        public int? Servings { get; set; }
    }
}
