namespace HouseholdManager.Database.DataModels
{

    public enum ItemCollectionStatus
    {
        Normal, Archived
    }
    public class ItemCollection
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public User Owner { get; set; } = default!;

        public int OwnerId { get; set; }

        public ItemCollectionStatus Status { get; set; }

        public List<ItemInstance> ItemInstances { get; set; } = [];

    }
}
