using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Services
{
    public interface IRecurrenceRuleService
    {
        void CreateRecurrenceRule(Item item, FrequencyType? frequencyType, int frequency = 1);

        void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequencyType, int frequency = 1);
    }
    public class RecurrenceRuleService : IRecurrenceRuleService
    {
        public void CreateRecurrenceRule(Item item, FrequencyType? frequencyType, int frequency = 1)
        {
            if (frequencyType is { } type)
            {
                var newRule = new RecurrenceRule { FrequencyType = type, Frequency = frequency };
                item.RecurrenceRule = newRule;
            }
        }

        public void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequencyType, int frequency = 1)
        {
            if (frequencyType is { } type)
            {
                rule.FrequencyType = type;
                rule.Frequency = frequency;
            }
        }
    }
}
