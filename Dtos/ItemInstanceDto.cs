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
    }
}
