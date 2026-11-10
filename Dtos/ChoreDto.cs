using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class ChoreDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int HouseholdId { get; set; }

        public int? UserId { get; set; }

        public FrequencyType? Frequency { get; set; }

        public int? Interval { get; set; }

        public Weekdays? Weekdays { get; set; }

        public static ChoreDto Map(Chore chore)
        {
            var result = new ChoreDto
            {
                Id = chore.Id,
                Name = chore.Name,
                HouseholdId = chore.HouseholdId,
                UserId = chore.UserId,
                Frequency = chore.RecurrenceRule?.Frequency,
                Interval = chore.RecurrenceRule?.Interval,
                Weekdays = chore.RecurrenceRule?.Weekdays
            };
            return result;
        }

        public static IEnumerable<ChoreDto> MapCollection(IEnumerable<Chore> chores)
        {
            foreach (var chore in chores)
            {
                yield return Map(chore);
            }
        }
    }
}
