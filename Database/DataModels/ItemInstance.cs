using HouseholdManager.Dtos;

namespace HouseholdManager.Database.DataModels
{
    public class ItemInstance
    {
        public int Id { get; set; }

        public DateTimeOffset PeriodStartDate { get; set; }

        public DateTimeOffset? DoneDate { get; set; }

        public bool IsDone => DoneDate != null;

        public Item Item { get; set; } = default!;
        public int ItemId { get; set; }

        public ItemInstanceDto Map()
        {
            return ItemInstanceDto.Map(this);
        }
    }
}
