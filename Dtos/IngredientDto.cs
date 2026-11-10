using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class IngredientDto
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public MeasurementUnit Unit { get; set; }

        public static IngredientDto Map(Ingredient ingredient)
        {
            var result = new IngredientDto
            {
                Id = ingredient.Id,
                RecipeId = ingredient.RecipeId,
                ProductId = ingredient.ProductId,
                ProductName = ingredient.Product.Name,
                Amount = ingredient.Amount,
                Unit = ingredient.Unit
            };
            return result;
        }

        public static IEnumerable<IngredientDto> MapCollection(IEnumerable<Ingredient> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                yield return Map(ingredient);
            }
        }
    }
}
