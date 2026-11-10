namespace HouseholdManager.Database.DataModels
{
    public enum FrequencyType
    {
        Daily, Weekly, Monthly, Yearly
    }

    [Flags]
    public enum Weekdays
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64
    }

    public class RecurrenceRule
    {
        public int Id { get; set; }

        public FrequencyType Frequency { get; set; }

        public int Interval { get; set; }

        public Weekdays Weekdays { get; set; }

        public Chore Chore { get; set; } = default!;
    }
}
