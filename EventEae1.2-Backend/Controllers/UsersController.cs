using Microsoft.AspNetCore.Mvc;
using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using EventEae1._2_Backend.Models;

namespace EventEae1._2_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOtpService _otpService;
        private readonly IAuditLogService _auditLogService;

        public UsersController(IUserService userService, IOtpService otpService, IAuditLogService auditLogService)
        {
            _userService = userService;
            _otpService = otpService;
            _auditLogService = auditLogService;

        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var result = await _userService.RegisterAsync(dto);
                return Ok(result); // Return user data (no password)
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            try
            {
                var loginResponse = await _userService.LoginAsync(dto);
                await _otpService.GenerateAndSendOTPAsync(dto.Email);
                return Ok(new { message = "OTP sent to your email." });
            }
            catch (Exception ex)
            {
                // Log failed login attempt due to exception
                await _auditLogService.LogAsync(null, AuditAction.Login, AuditStatus.Failed, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }


        // POST: api/Users/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                await _userService.ForgotPasswordAsync(dto);
                return Ok(new { message = "Password reset instructions sent (simulated)." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Users/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOtpDto dto)
        {
            try
            {
                // Verify OTP
                var isValid = await _otpService.VerifyOTPAsync(dto.Email, dto.Otp);
                if (!isValid)
                    return BadRequest(new { message = "Invalid or expired OTP." });

                // Credentials were already validated during login, so proceed with token generation
                var token = await _userService.GenerateTokenAsync(dto.Email);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get the user ID from JWT claims
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid? userId = string.IsNullOrEmpty(userIdString) ? null : Guid.Parse(userIdString);

                // Log successful logout
                await _auditLogService.LogAsync(userId, AuditAction.Logout, AuditStatus.Success);

                // Get the JWT token from Authorization header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
                var exp = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime;

                // If using refresh tokens, invalidate them here

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                // Log failed logout attempt
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid? userId = string.IsNullOrEmpty(userIdString) ? null : Guid.Parse(userIdString);
                await _auditLogService.LogAsync(userId, AuditAction.Logout, AuditStatus.Failed, ex.Message);

                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
