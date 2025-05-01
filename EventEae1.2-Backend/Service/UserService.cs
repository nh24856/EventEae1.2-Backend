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

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IConfiguration config)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
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

            // Get permissions
            var rolePermissions = user.Role != null
                ? await _userRepository.GetPermissionsByRoleAsync(user.Role)
                : new List<string>();

            var userPermissions = user.UserPermissions?
                .Select(up => up.Permission.Name)
                .ToList() ?? new List<string>();

            var allPermissions = rolePermissions
                .Union(userPermissions)
                .Distinct()
                .ToList();

            // Generate JWT token
            var token = GenerateJwtToken(user, allPermissions);

            return new LoginResponseDto
            {
                Token = token,
                Id = user.Id.ToString(),
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                Permissions = allPermissions,
                ExpiresIn = Convert.ToInt32(_config["Jwt:ExpireMinutes"]) * 60
            };
        }

        private string GenerateJwtToken(User user, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "user")
            };

            // Add permissions as claims
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

            // Generate password reset token (simplified)
            var resetToken = Guid.NewGuid().ToString();
            // In real implementation: store token and send email
        }
    }
}