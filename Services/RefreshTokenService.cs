using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HouseholdManager.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        public RefreshTokenService(HouseholdContext context, IOptions<JwtOptions> options, TimeProvider timeProvider)
        {
            _context = context;
            _options = options.Value;
            _timeProvider = timeProvider;
        }

        private readonly HouseholdContext _context;
        private readonly JwtOptions _options;
        private readonly TimeProvider _timeProvider;

        public async Task<string> CreateAsync(string userId)
        {
            var now = _timeProvider.GetUtcNow();

            var expiredTokens = await _context.RefreshTokens
                .Where(token => token.UserId == userId && token.ExpiresAt < now)
                .ToListAsync();
            _context.RefreshTokens.RemoveRange(expiredTokens);

            var rawToken = StageNewToken(userId, now);
            await _context.SaveChangesAsync();
            return rawToken;
        }

        public async Task<RotatedRefreshToken?> RotateAsync(string rawToken)
        {
            var tokenHash = Hash(rawToken);
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(existing => existing.TokenHash == tokenHash);
            if (token is null || token.RevokedAt is not null) return null;

            if (token.UsedAt is not null)
            {
                await RevokeAllAsync(token.UserId);
                return null;
            }

            var now = _timeProvider.GetUtcNow();
            if (token.ExpiresAt <= now) return null;

            token.UsedAt = now;
            var newRawToken = StageNewToken(token.UserId, now);
            await _context.SaveChangesAsync();
            return new RotatedRefreshToken(token.UserId, newRawToken);
        }

        public async Task RevokeAsync(string rawToken)
        {
            var tokenHash = Hash(rawToken);
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(existing => existing.TokenHash == tokenHash);
            if (token is null || token.RevokedAt is not null) return;

            token.RevokedAt = _timeProvider.GetUtcNow();
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllAsync(string userId)
        {
            var now = _timeProvider.GetUtcNow();
            var tokens = await _context.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens) token.RevokedAt = now;
            await _context.SaveChangesAsync();
        }

        private string StageNewToken(string userId, DateTimeOffset now)
        {
            var rawToken = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(64));
            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                TokenHash = Hash(rawToken),
                CreatedAt = now,
                ExpiresAt = now.AddDays(_options.RefreshTokenDays)
            });
            return rawToken;
        }

        private static string Hash(string rawToken)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
        }
    }
}
