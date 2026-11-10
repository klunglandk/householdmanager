using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Infrastructure
{
    public class RecurrenceGenerationOptions
    {
        public const string SectionName = "RecurrenceGeneration";

        [Range(1, 366)]
        public int HorizonDays { get; set; } = 28;

        public string TimeZoneId { get; set; } = "Europe/Oslo";

        public TimeOnly DailyRunTime { get; set; } = new TimeOnly(0, 5);

        public TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
        }
    }
}
