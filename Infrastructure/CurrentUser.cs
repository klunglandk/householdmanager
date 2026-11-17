using System.Security.Claims;

namespace HouseholdManager.Infrastructure
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal Principal =>
            _httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException("There is no active HTTP request.");

        public string UserId =>
            Principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("The token has no user id claim.");

        public int HouseholdId =>
            int.TryParse(Principal.FindFirstValue("householdId"), out var householdId)
                ? householdId
                : throw new InvalidOperationException("The token has no valid household id claim.");

        public bool IsAdmin => Principal.IsInRole("admin");
    }
}
