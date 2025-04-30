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

        public async Task<LoginResponseDto> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetUserWithPermissionsAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid email or password.");

            // Get role-based permissions
            var rolePermissions = user.Role != null
                ? await _userRepository.GetPermissionsByRoleAsync(user.Role)
                : new List<string>();

            // User-specific permissions
            var userPermissions = user.UserPermissions?
                .Select(up => up.Permission.Name)
                .ToList() ?? new List<string>();

            var allPermissions = rolePermissions
                .Union(userPermissions)
                .Distinct()
                .ToList();

            return new LoginResponseDto
            {
                Id = user.Id.ToString(),
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                Permissions = allPermissions
            };
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
