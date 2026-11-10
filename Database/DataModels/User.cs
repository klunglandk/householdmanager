using Microsoft.AspNetCore.Identity;
namespace HouseholdManager.Database.DataModels
{
    public class User : IdentityUser
    {

        public int HouseholdId { get; set; }

        public Household Household { get; set; } = default!;

        public List<Chore> Chores { get; set; } = [];

    }
}
