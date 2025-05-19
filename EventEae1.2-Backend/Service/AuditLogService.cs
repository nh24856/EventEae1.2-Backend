// Services/AuditLogService.cs
using EventEae1._2_Backend.Data;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            AppDbContext context, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<AuditLogService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogAsync(string userId, string userEmail, string action,
                                 string entityType = null, string entityId = null,
                                 object oldValues = null, object newValues = null)
        {
            try
            {
                var log = new AuditLog
                {
                    UserId = userId ?? "unknown",
                    UserEmail = userEmail ?? "unknown",
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    OldValues = oldValues != null ? JsonConvert.SerializeObject(oldValues) : null,
                    NewValues = newValues != null ? JsonConvert.SerializeObject(newValues) : null,
                    IpAddress = GetClientIpAddress(),
                    Timestamp = DateTime.UtcNow
                };

                await _context.AuditLogs.AddAsync(log);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Audit log created: {Action} by {UserEmail} ({UserId})", 
                    action, userEmail, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log for action: {Action}, User: {UserEmail}", 
                    action, userEmail);
                // Don't throw - audit logging failure shouldn't break the main operation
            }
        }

        public async Task LogWithContextAsync(string action, string entityType = null, string entityId = null,
                                             object oldValues = null, object newValues = null)
        {
            var context = _httpContextAccessor.HttpContext;
            var userId = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var userEmail = context?.User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? "unknown";

            await LogAsync(userId, userEmail, action, entityType, entityId, oldValues, newValues);
        }

        private string GetClientIpAddress()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return "unknown";

                // Check for forwarded IP first (in case of reverse proxy)
                var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                return context.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get client IP address");
                return "unknown";
            }
        }
    }
}