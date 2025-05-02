using System.Collections.Concurrent;

namespace EventEae1._2_Backend.Interfaces
{
    public interface ITokenBlacklistService
    {
        void BlacklistToken(string tokenId, TimeSpan expiry);
        bool IsTokenBlacklisted(string tokenId);
    }

    // TokenBlacklistService.cs
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

        public void BlacklistToken(string tokenId, TimeSpan expiry)
        {
            _blacklistedTokens.TryAdd(tokenId, DateTime.UtcNow.Add(expiry));
        }

        public bool IsTokenBlacklisted(string tokenId)
        {
            if (_blacklistedTokens.TryGetValue(tokenId, out var expiryDate))
            {
                if (expiryDate > DateTime.UtcNow) return true;
                _blacklistedTokens.TryRemove(tokenId, out _);
            }
            return false;
        }
    }
}
