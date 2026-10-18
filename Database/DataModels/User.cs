namespace HouseholdManager.Database.DataModels
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public List<ItemCollection> ItemCollections { get; set; } = [];

    }
}
