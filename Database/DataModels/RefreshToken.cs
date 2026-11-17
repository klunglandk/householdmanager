namespace HouseholdManager.Database.DataModels
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string TokenHash { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset? UsedAt { get; set; }

        public DateTimeOffset? RevokedAt { get; set; }
    }
}
