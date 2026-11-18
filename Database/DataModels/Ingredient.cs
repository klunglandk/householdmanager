namespace HouseholdManager.Database.DataModels
{
    public class Ingredient
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; } = default!;

        public int ProductId { get; set; }

        public Product Product { get; set; } = default!;

        public decimal Amount { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
