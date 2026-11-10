namespace HouseholdManager.Database.DataModels
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int HouseholdId { get; set; }

        public Household Household { get; set; } = default!;

        public List<Chore> Chores { get; set; } = [];

    }
}
