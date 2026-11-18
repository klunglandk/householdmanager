namespace HouseholdManager.Services
{
    public interface IRefreshTokenService
    {
        Task<string> CreateAsync(string userId);

        Task<RotatedRefreshToken?> RotateAsync(string rawToken);

        Task RevokeAsync(string rawToken);

        Task RevokeAllAsync(string userId);
    }
}
