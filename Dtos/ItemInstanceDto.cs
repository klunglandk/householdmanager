using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class ItemInstanceDto
    {
        public int Id { get; set; }
        public DateTimeOffset PeriodStartDate { get; set; }

        public DateTimeOffset? DoneDate { get; set; }

        public bool IsDone => DoneDate != null;

        public string ItemName { get; set; } = string.Empty;

        public FrequencyType? FrequencyType { get; set; }

        public int? Frequency {  get; set; }

        public static ItemInstanceDto Map(ItemInstance itemInstance)
        {
            var result = new ItemInstanceDto()
            {
                Id = itemInstance.Id,
                PeriodStartDate = itemInstance.PeriodStartDate,
                DoneDate = itemInstance.DoneDate,
                ItemName = itemInstance.Item.Name,
                FrequencyType = itemInstance.Item.RecurrenceRule?.FrequencyType,
                Frequency = itemInstance.Item.RecurrenceRule?.Frequency
            };
            return result;
        }

        public static IEnumerable<ItemInstanceDto> MapCollection(IEnumerable<ItemInstance> itemInstance)
        {
            foreach(var item in itemInstance)
            {
                yield return Map(item);
            }
        }
    }
}
