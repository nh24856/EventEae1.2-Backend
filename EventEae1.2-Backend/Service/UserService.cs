using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EventEae1._2_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IAuditLogService _auditLogService;

        public UserService(IUserRepository userRepository, IMapper mapper, 
            IConfiguration config, IAuditLogService auditLogService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
            _auditLogService = auditLogService;
        }

        public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
        {
            if (dto.Role == "manager" && string.IsNullOrWhiteSpace(dto.Organization))
                throw new Exception("Organization is required for managers.");

            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)

                {
                     await _auditLogService.LogAsync("anonymous", dto.Email, "RegistrationFailed",
                         "User", null, null, new { Reason = "Email already exists" });
                     throw new Exception("Email already exists."); 
                }

            var user = _mapper.Map<User>(dto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.AddUserAsync(user);

            await _auditLogService.LogAsync(user.Id.ToString(), user.Email, "UserRegistered",
                "User", user.Id.ToString(), null, new { Role = user.Role, Status = user.Status });

            return _mapper.Map<UserDto>(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetUserWithPermissionsAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                {
                     await _auditLogService.LogAsync("anonymous", dto.Email, "LoginFailed");
                     throw new Exception("Invalid email or password.");
                 }

            await _auditLogService.LogAsync(user.Id.ToString(), user.Email, "LoginSuccess",
                "User", user.Id.ToString());

            var allPermissions = await GetAllPermissionsAsync(user);
            var token = GenerateJwtToken(user, allPermissions);

            return CreateLoginResponse(user, token, allPermissions);
        }

        public async Task<LoginResponseDto> GenerateTokenAsync(string email)
        {
            var user = await _userRepository.GetUserWithPermissionsAsync(email);
            if (user == null)
            {
                await _auditLogService.LogAsync("system", email, "TokenGenerationFailed");
                throw new Exception("User not found.");
            }

            await _auditLogService.LogAsync(user.Id.ToString(), user.Email, "TokenGenerated", "User",
                user.Id.ToString());

            var allPermissions = await GetAllPermissionsAsync(user);
            var token = GenerateJwtToken(user, allPermissions);

            return CreateLoginResponse(user, token, allPermissions);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                await _auditLogService.LogAsync("system", dto.Email, "PasswordResetRequestFailed");
                throw new Exception("Email not found.");
            }

            // Generate password reset token (simplified)
            var resetToken = Guid.NewGuid().ToString();

            await _auditLogService.LogAsync(user.Id.ToString(), user.Email,
                "PasswordResetRequested", "User", user.Id.ToString());
            // In real implementation: store token and send email
        }

        // Helpers

        private async Task<List<string>> GetAllPermissionsAsync(User user)
        {
            var rolePermissions = user.Role != null
                ? await _userRepository.GetPermissionsByRoleAsync(user.Role)
                : new List<string>();

            var userPermissions = user.UserPermissions?
                .Select(up => up.Permission.Name)
                .ToList() ?? new List<string>();

            return rolePermissions
                .Union(userPermissions)
                .Distinct()
                .ToList();
        }

        private string GenerateJwtToken(User user, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 👈 ADD THIS LINE ✅
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "user")
            };

            permissions.ForEach(p => claims.Add(new Claim("permission", p)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private LoginResponseDto CreateLoginResponse(User user, string token, List<string> permissions)
        {
            return new LoginResponseDto
            {
                Token = token,
                Id = user.Id.ToString(),
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                Permissions = permissions,
                ExpiresIn = Convert.ToInt32(_config["Jwt:ExpireMinutes"]) * 60
            };
        }
    }
}
