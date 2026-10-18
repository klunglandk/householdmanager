using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Services
{
    public interface IUserService
    {
        User? GetUser(int userId);
    }
    public class UserService : IUserService
    {
        public UserService(HouseholdContext context) {
            _context = context;
        }

        private readonly HouseholdContext _context;
        public User? GetUser(int userId)
        {
            return _context.Users.FirstOrDefault(user => user.Id == userId);
        }
    }
}
