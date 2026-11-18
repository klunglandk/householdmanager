namespace HouseholdManager.Database.DataModels
{
    public class Recipe
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Instructions { get; set; } = string.Empty;

        public int? Servings { get; set; }

        public string? ImagePath { get; set; }

        public int HouseholdId { get; set; }

        public Household Household { get; set; } = default!;

        public List<Ingredient> Ingredients { get; set; } = [];
    }
}
