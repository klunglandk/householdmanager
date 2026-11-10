using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class RecipeDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Instructions { get; set; } = string.Empty;

        public int? Servings { get; set; }

        public bool HasImage { get; set; }

        public int HouseholdId { get; set; }

        public List<IngredientDto> Ingredients { get; set; } = [];

        public static RecipeDto Map(Recipe recipe)
        {
            var result = new RecipeDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Instructions = recipe.Instructions,
                Servings = recipe.Servings,
                HasImage = recipe.ImagePath != null,
                HouseholdId = recipe.HouseholdId,
                Ingredients = IngredientDto.MapCollection(recipe.Ingredients).ToList()
            };
            return result;
        }

        public static IEnumerable<RecipeDto> MapCollection(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                yield return Map(recipe);
            }
        }
    }
}
