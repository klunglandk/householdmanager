using HouseholdManager.Database.DataModels;

namespace HouseholdManager.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public int HouseholdId { get; set; }

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public static UserDto Map(User user, string role)
        {
            var result = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                HouseholdId = user.HouseholdId,
                Role = role,
                IsActive = user.LockoutEnd is null || user.LockoutEnd <= DateTimeOffset.UtcNow
            };
            return result;
        }
    }
}
