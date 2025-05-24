using EventEae1._2_Backend.Interfaces;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            // Load SMTP settings from configuration (appsettings.json)
            _smtpServer = configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
            _smtpUser = configuration["EmailSettings:SmtpUser"];
            _smtpPass = configuration["EmailSettings:SmtpPass"];
            _fromEmail = configuration["EmailSettings:FromEmail"];
        }

        public async Task SendOTPAsync(string email, string otp)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = "Your OTP Code",
                Body = $"Your OTP code is {otp}. It is valid for 5 minutes.",
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendManagerApprovalEmailAsync(string email, bool isApproved)
        {
            var subject = isApproved ? "Manager Account Approved" : "Manager Account Rejected";
            var body = isApproved
                ? "Dear Manager,\n\nYour account has been approved. You can now access the system.\n\nBest regards,\nEventEase Team"
                : "Dear Manager,\n\nYour account has been rejected. Please contact the administrator for more details.\n\nBest regards,\nEventEase Team";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }

    }
}