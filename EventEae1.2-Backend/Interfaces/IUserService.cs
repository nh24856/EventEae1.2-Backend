using EventEae1._2_Backend.DTOs;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IUserService
    {


        Task<UserDto> RegisterAsync(RegisterUserDto dto);
        Task<string> LoginAsync(LoginUserDto dto);
        Task<string> GenerateTokenAsync(string email);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
    }
}
