using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;
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
        public HouseholdService(HouseholdContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        private readonly HouseholdContext _context;
        private readonly IFileStorage _fileStorage;

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

            var storedFileNames = _context.Recipes
                .Where(recipe => recipe.HouseholdId == householdId && recipe.ImagePath != null)
                .Select(recipe => recipe.ImagePath!)
                .ToList()
                .Concat(_context.SchoolPlans.Where(plan => plan.HouseholdId == householdId).Select(plan => plan.StoredFileName))
                .ToList();

            _context.Households.Remove(household);
            _context.SaveChanges();

            foreach (var storedFileName in storedFileNames)
            {
                _fileStorage.Delete(storedFileName);
            }
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
