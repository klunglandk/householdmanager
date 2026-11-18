namespace HouseholdManager.Database.DataModels
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int HouseholdId { get; set; }

        public Household Household { get; set; } = default!;

        public List<Ingredient> Ingredients { get; set; } = [];

        public List<ShoppingItem> ShoppingItems { get; set; } = [];
    }
}
