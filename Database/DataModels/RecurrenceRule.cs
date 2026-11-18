namespace HouseholdManager.Database.DataModels
{
    public class RecurrenceRule
    {
        public int Id { get; set; }

        public FrequencyType Frequency { get; set; }

        public int Interval { get; set; }

        public Weekdays Weekdays { get; set; }

        public DateOnly StartDate { get; set; }

        public Chore Chore { get; set; } = default!;
    }
}
