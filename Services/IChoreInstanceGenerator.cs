using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Services
{
    public interface IChoreInstanceGenerator
    {
        DateOnly GetToday();

        int GenerateInstances(DateOnly rangeStart, DateOnly rangeEndExclusive);

        int StageInstancesForChore(Chore chore, DateOnly fromDate, Chore? replacedChore = null);

        int StageRemovalOfOpenInstances(Chore chore, DateOnly fromDate);
    }
}
