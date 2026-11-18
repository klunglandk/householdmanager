using HouseholdManager.Database.DataModels;
using HouseholdManager.Models;

namespace HouseholdManager.Services
{
    public interface IHouseholdService
    {
        Household CreateHousehold(CreateHouseholdModel model);

        Household? GetHousehold(int householdId);

        List<Household> GetAllHouseholds();

        Household? UpdateHousehold(int householdId, UpdateHouseholdModel model);

        bool DeleteHousehold(int householdId);
    }
}
