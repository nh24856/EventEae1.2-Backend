using AutoMapper;
using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;

namespace EventEae1._2_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
        {
            if (dto.Role == "manager" && string.IsNullOrWhiteSpace(dto.Organization))
                throw new Exception("Organization is required for managers.");

            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email already exists.");

            var user = _mapper.Map<User>(dto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.AddUserAsync(user);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid email or password.");

            // Later: Generate JWT Token
            return "DummyTokenForNow";
        }

        public async Task<string> GenerateTokenAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found.");

            // Later: Generate JWT Token
            return "DummyTokenForNow";
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Email not found.");

            // send email here (mock for now)
        }
    }
}
