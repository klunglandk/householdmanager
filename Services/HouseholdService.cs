using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using System.ComponentModel.DataAnnotations;

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

    public class HouseholdService : IHouseholdService
    {
        public HouseholdService(HouseholdContext context)
        {
            _context = context;
        }

        private readonly HouseholdContext _context;

        public Household CreateHousehold(CreateHouseholdModel model)
        {
            var result = _context.Households.Add(new Household { Name = model.Name });
            _context.SaveChanges();
            return result.Entity;
        }

        public Household? GetHousehold(int householdId)
        {
            return _context.Households.FirstOrDefault(household => household.Id == householdId);
        }

        public List<Household> GetAllHouseholds()
        {
            return _context.Households.ToList();
        }

        public Household? UpdateHousehold(int householdId, UpdateHouseholdModel model)
        {
            if (GetHousehold(householdId) is not { } household) return null;
            if (model.Name is { } name) { household.Name = name; }
            _context.SaveChanges();
            return household;
        }

        public bool DeleteHousehold(int householdId)
        {
            if (GetHousehold(householdId) is not { } household) return false;

            _context.Households.Remove(household);
            _context.SaveChanges();
            return true;
        }
    }

    public class CreateHouseholdModel
    {
        [Required, MinLength(2), MaxLength(128)]
        public required string Name { get; set; }
    }

    public class UpdateHouseholdModel
    {
        [MinLength(2), MaxLength(128)]
        public string? Name { get; set; }
    }
}
