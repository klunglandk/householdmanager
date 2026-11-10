using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Services
{
    public static class RecurrenceCalculator
    {
        public static List<DateOnly> GetOccurrences(RecurrenceRule rule, DateOnly rangeStart, DateOnly rangeEndExclusive)
        {
            return GetOccurrences(rule.Frequency, rule.Interval, rule.Weekdays, rule.StartDate, rangeStart, rangeEndExclusive);
        }

        public static List<DateOnly> GetOccurrences(FrequencyType frequency, int interval, Weekdays weekdays, DateOnly startDate, DateOnly rangeStart, DateOnly rangeEndExclusive)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(interval, 1);

            var occurrences = new List<DateOnly>();
            var firstDate = rangeStart > startDate ? rangeStart : startDate;

            for (var date = firstDate; date < rangeEndExclusive; date = date.AddDays(1))
            {
                if (Matches(frequency, interval, weekdays, startDate, date))
                {
                    occurrences.Add(date);
                }
            }

            return occurrences;
        }

        private static bool Matches(FrequencyType frequency, int interval, Weekdays weekdays, DateOnly startDate, DateOnly date)
        {
            return frequency switch
            {
                FrequencyType.Daily => MatchesDaily(interval, startDate, date),
                FrequencyType.Weekly => MatchesWeekly(interval, weekdays, startDate, date),
                FrequencyType.Monthly => MatchesMonthly(interval, startDate, date),
                FrequencyType.Yearly => MatchesYearly(interval, startDate, date),
                _ => false
            };
        }

        private static bool MatchesDaily(int interval, DateOnly startDate, DateOnly date)
        {
            return (date.DayNumber - startDate.DayNumber) % interval == 0;
        }

        private static bool MatchesWeekly(int interval, Weekdays weekdays, DateOnly startDate, DateOnly date)
        {
            var weeksSinceStart = (GetMondayOfWeek(date).DayNumber - GetMondayOfWeek(startDate).DayNumber) / 7;
            return weeksSinceStart % interval == 0 && weekdays.HasFlag(GetWeekdayFlag(date.DayOfWeek));
        }

        private static bool MatchesMonthly(int interval, DateOnly startDate, DateOnly date)
        {
            var monthsSinceStart = (date.Year - startDate.Year) * 12 + date.Month - startDate.Month;
            return monthsSinceStart % interval == 0 && date.Day == GetClampedDay(startDate.Day, date.Year, date.Month);
        }

        private static bool MatchesYearly(int interval, DateOnly startDate, DateOnly date)
        {
            var yearsSinceStart = date.Year - startDate.Year;
            return yearsSinceStart % interval == 0
                && date.Month == startDate.Month
                && date.Day == GetClampedDay(startDate.Day, date.Year, date.Month);
        }

        private static int GetClampedDay(int day, int year, int month)
        {
            return Math.Min(day, DateTime.DaysInMonth(year, month));
        }

        private static DateOnly GetMondayOfWeek(DateOnly date)
        {
            var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
            return date.AddDays(-daysSinceMonday);
        }

        private static Weekdays GetWeekdayFlag(DayOfWeek dayOfWeek)
        {
            var daysSinceMonday = ((int)dayOfWeek + 6) % 7;
            return (Weekdays)(1 << daysSinceMonday);
        }
    }
}
