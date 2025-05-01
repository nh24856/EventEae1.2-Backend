namespace EventEae1._2_Backend.DTOs
{
    // For returning user data
    public class UserDto
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? Organization { get; set; }
        public string Status { get; set; }
    }

    // For user registration (sign up)
    public class RegisterUserDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // User must choose role during signup (user or manager)
        public string Role { get; set; } = "user";

        // Only required if role == "manager"
        public string? Organization { get; set; }
    }

    // For user login
    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // For forgot password
    public class ForgotPasswordDto
    {
        public string Email { get; set; }
    }

    public class LoginResponseDto
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public int ExpiresIn { get; set; }
        public string Token { get; set; }
        public List<string> Permissions { get; set; }
    }

    public class UpdatePermissionsDto
    {
        public List<string> Permissions { get; set; }
    }

    public class ManagerApprovalDto
    {
        public bool Approved { get; set; }
    }
    public class PendingManagerDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string OrganizationName { get; set; }
        public DateTime RequestedAt { get; set; }
    }



    //for otp verification
    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
