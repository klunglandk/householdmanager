namespace HouseholdManager.Infrastructure
{
    public interface ICurrentUser
    {
        string UserId { get; }
        int HouseholdId { get; }
        bool IsAdmin { get; }
    }
}
