using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HouseholdManager.Services
{
    public interface IRecurrenceRuleService
    {
        void CreateRecurrenceRule(Chore chore, FrequencyType? frequency, int? interval, Weekdays? weekdays);

        void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequency, int? interval, Weekdays? weekdays);

        void DeleteRecurrenceRule(RecurrenceRule rule);
    }
    public class RecurrenceRuleService : IRecurrenceRuleService
    {
        public RecurrenceRuleService(HouseholdContext context)
        {
            _context = context;
        }

        private readonly HouseholdContext _context;

        public void CreateRecurrenceRule(Chore chore, FrequencyType? frequency, int? interval, Weekdays? weekdays)
        {
            if (frequency is { } type)
            {
                var newRule = new RecurrenceRule { Frequency = type, Interval = interval ?? 1, Weekdays = weekdays ?? Weekdays.None };
                chore.RecurrenceRule = newRule;
            }
        }

        public void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequency, int? interval, Weekdays? weekdays)
        {
            if (frequency is { } type)
            {
                rule.Frequency = type;
                rule.Interval = interval ?? rule.Interval;
                rule.Weekdays = weekdays ?? rule.Weekdays;
            }
        }

        public void DeleteRecurrenceRule(RecurrenceRule rule)
        {
            _context.RecurrenceRules.Remove(rule);
        }
    }
}
