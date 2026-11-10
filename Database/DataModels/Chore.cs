namespace HouseholdManager.Database.DataModels
{
    public class Chore
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int HouseholdId { get; set; }

        public Household Household { get; set; } = default!;

        public int? UserId { get; set; }

        public User? User { get; set; }

        public int? RecurrenceRuleId { get; set; }

        public RecurrenceRule? RecurrenceRule { get; set; }

        public List<ChoreInstance> ChoreInstances { get; set; } = [];

    }
}
