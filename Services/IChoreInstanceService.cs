using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;

namespace HouseholdManager.Services
{
    public interface IChoreInstanceService
    {
        ChoreInstance? GetChoreInstance(int choreInstanceId);

        List<ChoreInstance> GetAllChoreInstances(DateTimeOffset? from, DateTimeOffset? to, string? userId, bool onlyOpen);

        ChoreInstance? CreateChoreInstance(CreateChoreInstanceModel model);

        ChoreInstance CreateChoreInstanceWithNewChore(CreateChoreInstanceWithNewChoreModel model);

        ChoreInstance? MarkAsDone(int choreInstanceId);

        ChoreInstance? ClearDoneDate(int choreInstanceId);

        ChoreInstance? MarkAsSkipped(int choreInstanceId);

        ChoreInstance? ClearSkippedDate(int choreInstanceId);

        bool DeleteChoreInstance(int choreInstanceId);
    }
}
