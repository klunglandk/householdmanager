using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class ChoreInstanceDto
    {
        public int Id { get; set; }

        public DateTimeOffset PeriodStartDate { get; set; }

        public DateTimeOffset? DoneDate { get; set; }

        public bool IsDone => DoneDate != null;

        public DateTimeOffset? SkippedDate { get; set; }

        public bool IsSkipped => SkippedDate != null;

        public int ChoreId { get; set; }

        public string ChoreName { get; set; } = string.Empty;

        public int? UserId { get; set; }

        public FrequencyType? Frequency { get; set; }

        public int? Interval { get; set; }

        public Weekdays? Weekdays { get; set; }

        public DateOnly? StartDate { get; set; }

        public static ChoreInstanceDto Map(ChoreInstance choreInstance)
        {
            var result = new ChoreInstanceDto()
            {
                Id = choreInstance.Id,
                PeriodStartDate = choreInstance.PeriodStartDate,
                DoneDate = choreInstance.DoneDate,
                SkippedDate = choreInstance.SkippedDate,
                ChoreId = choreInstance.ChoreId,
                ChoreName = choreInstance.Chore.Name,
                UserId = choreInstance.Chore.UserId,
                Frequency = choreInstance.Chore.RecurrenceRule?.Frequency,
                Interval = choreInstance.Chore.RecurrenceRule?.Interval,
                Weekdays = choreInstance.Chore.RecurrenceRule?.Weekdays,
                StartDate = choreInstance.Chore.RecurrenceRule?.StartDate
            };
            return result;
        }

        public static IEnumerable<ChoreInstanceDto> MapCollection(IEnumerable<ChoreInstance> choreInstances)
        {
            foreach (var instance in choreInstances)
            {
                yield return Map(instance);
            }
        }
    }
}
