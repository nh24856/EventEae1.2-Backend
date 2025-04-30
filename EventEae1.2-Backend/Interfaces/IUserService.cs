using EventEae1._2_Backend.DTOs;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(RegisterUserDto dto);
        Task<LoginResponseDto> LoginAsync(LoginUserDto dto);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
    }
}
