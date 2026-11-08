using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class ItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public FrequencyType? FrequencyType { get; set; }
        
        public int? Frequency { get; set; }

        public int? Interval { get; set; }

        public int ItemCollectionId { get; set; }

        public static ItemDto Map(Item item)
        {
            var result = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                FrequencyType = item.RecurrenceRule?.FrequencyType,
                Frequency = item.RecurrenceRule?.Frequency,
                Interval = item.RecurrenceRule?.Interval,
                ItemCollectionId = item.ItemCollectionId
            };
            return result;
        }

        public static IEnumerable<ItemDto> MapItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                yield return Map(item);
            }
        }
    }
}
