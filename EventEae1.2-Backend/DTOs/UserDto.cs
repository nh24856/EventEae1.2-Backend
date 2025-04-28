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
}
