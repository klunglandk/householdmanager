using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Services
{
    public interface IRecurrenceRuleService
    {
        void CreateRecurrenceRule(Chore chore, FrequencyType? frequency, int? interval, Weekdays? weekdays, DateOnly? startDate);

        void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequency, int? interval, Weekdays? weekdays, DateOnly? startDate);

        void DeleteRecurrenceRule(RecurrenceRule rule);
    }
    public class RecurrenceRuleService : IRecurrenceRuleService
    {
        public RecurrenceRuleService(HouseholdContext context)
        {
            _context = context;
        }

        private const int AllWeekdaysMask = 127;

        private readonly HouseholdContext _context;

        public void CreateRecurrenceRule(Chore chore, FrequencyType? frequency, int? interval, Weekdays? weekdays, DateOnly? startDate)
        {
            if (frequency is { } type)
            {
                var resolvedWeekdays = ResolveWeekdays(type, weekdays, Weekdays.None);
                var resolvedStartDate = startDate ?? throw new ValidationException("StartDate is required when a recurrence is set.");
                var newRule = new RecurrenceRule { Frequency = type, Interval = interval ?? 1, Weekdays = resolvedWeekdays, StartDate = resolvedStartDate };
                chore.RecurrenceRule = newRule;
            }
        }

        public void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequency, int? interval, Weekdays? weekdays, DateOnly? startDate)
        {
            if (frequency is { } type)
            {
                var resolvedWeekdays = ResolveWeekdays(type, weekdays, rule.Weekdays);
                rule.Frequency = type;
                rule.Interval = interval ?? rule.Interval;
                rule.Weekdays = resolvedWeekdays;
                rule.StartDate = startDate ?? rule.StartDate;
            }
        }

        public void DeleteRecurrenceRule(RecurrenceRule rule)
        {
            _context.RecurrenceRules.Remove(rule);
        }

        private static Weekdays ResolveWeekdays(FrequencyType frequency, Weekdays? requestedWeekdays, Weekdays currentWeekdays)
        {
            if (requestedWeekdays is { } requested && ((int)requested & ~AllWeekdaysMask) != 0)
            {
                throw new ValidationException("Weekdays contains an invalid value.");
            }

            if (frequency != FrequencyType.Weekly)
            {
                if (requestedWeekdays is { } and not Weekdays.None)
                {
                    throw new ValidationException("Weekdays can only be used with a weekly frequency.");
                }
                return Weekdays.None;
            }

            var resolvedWeekdays = requestedWeekdays ?? currentWeekdays;
            if (resolvedWeekdays == Weekdays.None)
            {
                throw new ValidationException("A weekly frequency requires at least one weekday.");
            }
            return resolvedWeekdays;
        }
    }
}
