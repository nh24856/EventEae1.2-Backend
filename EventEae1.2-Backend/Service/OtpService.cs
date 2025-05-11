using EventEae1._2_Backend.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EventEae1._2_Backend.Service
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public OtpService(IMemoryCache memoryCache, IEmailService emailService, IUserRepository userRepository)
        {
            _memoryCache = memoryCache;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateAndSendOTPAsync(string email)
        {
            // Check if user exists
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                throw new Exception("User with this email does not exist.");

            // Generate 6-digit OTP
            var otp = new Random().Next(10000, 99999).ToString();

            // Store OTP in memory cache with 5-minute expiration
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _memoryCache.Set(email, otp, cacheEntryOptions);

            // Send OTP via email
            await _emailService.SendOTPAsync(email, otp);

            return "OTP sent successfully.";
        }

        public async Task<bool> VerifyOTPAsync(string email, string otp)
        {
            // Check if OTP exists in cache and matches
            if (_memoryCache.TryGetValue(email, out string storedOTP) && storedOTP == otp)
            {
                // Remove OTP from cache after successful verification
                _memoryCache.Remove(email);
                return true;
            }
            return false;
        }
    }
}
