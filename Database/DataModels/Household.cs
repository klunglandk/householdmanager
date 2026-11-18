namespace HouseholdManager.Database.DataModels
{
    public class Household
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<User> Users { get; set; } = [];

        public List<Chore> Chores { get; set; } = [];

        public List<Recipe> Recipes { get; set; } = [];

        public List<Product> Products { get; set; } = [];

        public List<SchoolPlan> SchoolPlans { get; set; } = [];
    }
}
