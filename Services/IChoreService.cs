using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;

namespace HouseholdManager.Services
{
    public interface IChoreService
    {
        Chore? GetChore(int choreId);

        List<Chore> GetAllChores(string? userId);

        Chore CreateChore(CreateChoreModel model);

        Chore? UpdateChore(int choreId, UpdateChoreModel model);

        Chore? UpdateFutureChore(int choreId, UpdateChoreModel model);

        Chore? EndChore(int choreId);

        bool DeleteChore(int choreId);
    }
}
