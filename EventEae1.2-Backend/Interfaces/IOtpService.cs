namespace EventEae1._2_Backend.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateAndSendOTPAsync(string email);
        Task<bool> VerifyOTPAsync(string email, string otp);
    }
}
