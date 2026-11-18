using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Services
{
    public interface IRecurrenceRuleService
    {
        void CreateRecurrenceRule(Chore chore, FrequencyType? frequency, int? interval, Weekdays? weekdays, DateOnly? startDate);

        void UpdateRecurrenceRule(RecurrenceRule rule, FrequencyType? frequency, int? interval, Weekdays? weekdays, DateOnly? startDate);

        void DeleteRecurrenceRule(RecurrenceRule rule);
    }
}
