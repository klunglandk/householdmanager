namespace HouseholdManager.Database.DataModels
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public RecurrenceRule? RecurrenceRule { get; set; }

        public int? RecurrenceRuleId { get; set; }

        public List<ItemInstance> ItemInstances { get; set; } = []; 

    }
}
