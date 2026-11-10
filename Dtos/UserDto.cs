using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public int HouseholdId { get; set; }

        public static UserDto Map(User user)
        {
            var result = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                HouseholdId = user.HouseholdId
            };
            return result;
        }

        public static IEnumerable<UserDto> MapCollection(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                yield return Map(user);
            }
        }
    }
}
