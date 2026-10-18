namespace HouseholdManager.Database.DataModels
{
    public enum FrequencyType
    {
        Daily, Weekly, Monthly, Yearly
    }
    
    public class RecurrenceRule
    {
        public int Id { get; set; }

        public FrequencyType FrequencyType { get; set; }

        public int Frequency { get; set; }

        public Item Item { get; set; } = default!;
    }
}
