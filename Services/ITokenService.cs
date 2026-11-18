namespace HouseholdManager.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(string userId, string userName, string userRole, int householdId);
    }
}
