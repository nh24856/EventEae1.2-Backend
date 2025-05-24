namespace EventEae1._2_Backend.Interfaces
{
    public interface IEmailService
    {
        Task SendOTPAsync(string email, string otp);
        Task SendManagerApprovalEmailAsync(string email, bool isApproved);
    }
}
