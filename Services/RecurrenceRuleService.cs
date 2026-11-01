using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HouseholdManager.Services
{
    public interface IRecurrenceRuleService
    {
        void CreateRecurrenceRule(Item item, FrequencyType? frequencyType, int? frequency);

        void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequencyType, int? frequency);

        void DeleteRecurrenceRule(RecurrenceRule rule);
    }
    public class RecurrenceRuleService : IRecurrenceRuleService
    {
        public RecurrenceRuleService(HouseholdContext context)
        {
            _context = context;
        }

        private readonly HouseholdContext _context;

        public void CreateRecurrenceRule(Item item, FrequencyType? frequencyType, int? frequency)
        {
            if (frequencyType is { } type)
            {
                var newRule = new RecurrenceRule { FrequencyType = type, Frequency = frequency ?? 1 };
                item.RecurrenceRule = newRule;
            }
        }

        public void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequencyType, int? frequency)
        {
            if (frequencyType is { } type)
            {
                rule.FrequencyType = type;
                rule.Frequency = frequency ?? rule.Frequency;
            }
        }

        public void DeleteRecurrenceRule(RecurrenceRule rule)
        {
            _context.RecurrenceRules.Remove(rule);
        }
    }
}
