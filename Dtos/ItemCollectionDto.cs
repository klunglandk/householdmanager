using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class ItemCollectionDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int OwnerId { get; set; }

        public ItemCollectionStatus Status { get; set; }

        public static ItemCollectionDto Map(ItemCollection itemCollection)
        {
            var result = new ItemCollectionDto
            {
                Id = itemCollection.Id,
                Name = itemCollection.Name,
                OwnerId = itemCollection.OwnerId,
                Status = itemCollection.Status
            };
            return result;
        }

        public static IEnumerable<ItemCollectionDto> MapCollection(IEnumerable<ItemCollection> itemCollections)
        {
            foreach (var collection in itemCollections)
            {
                yield return Map(collection);
            }
        }
    }
}
