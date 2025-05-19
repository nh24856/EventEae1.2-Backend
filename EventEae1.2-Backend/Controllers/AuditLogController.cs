using EventEae1._2_Backend.Data;
using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    public class AuditLogController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(AppDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLogs([FromQuery] AuditLogFilterDto filter)
        {
            // Log the audit log access
            var currentUserEmail = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? "unknown";
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            
            await _auditLogService.LogAsync(currentUserId, currentUserEmail, "AuditLogsAccessed", 
                "AuditLog", null, null, new { Filter = filter });

            var query = _context.AuditLogs.AsQueryable();

            // Apply filters
            if (filter.FromDate.HasValue)
                query = query.Where(l => l.Timestamp >= filter.FromDate);

            if (filter.ToDate.HasValue)
                query = query.Where(l => l.Timestamp <= filter.ToDate);

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(l => l.UserId.Contains(filter.UserId));

            if (!string.IsNullOrEmpty(filter.UserEmail))
                query = query.Where(l => l.UserEmail.Contains(filter.UserEmail));

            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(l => l.Action.Contains(filter.Action));

            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(l => l.EntityType.Contains(filter.EntityType));

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var logs = await query.OrderByDescending(l => l.Timestamp)
                                .Skip((filter.PageNumber - 1) * filter.PageSize)
                                .Take(filter.PageSize)
                                .Select(l => new AuditLogDto
                                {
                                    Id = l.Id,
                                    UserId = l.UserId,
                                    UserEmail = l.UserEmail,
                                    Action = l.Action,
                                    EntityType = l.EntityType,
                                    EntityId = l.EntityId,
                                    OldValues = l.OldValues,
                                    NewValues = l.NewValues,
                                    Timestamp = l.Timestamp,
                                    IpAddress = l.IpAddress
                                })
                                .ToListAsync();

            var response = new AuditLogResponseDto
            {
                Logs = logs,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };

            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserLogs(string userId, [FromQuery] AuditLogFilterDto filter)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserEmail = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? "unknown";

            // Users can only view their own logs unless they're Admin
            if (currentUserRole != "Admin" && currentUserId != userId)
            {
                await _auditLogService.LogAsync(currentUserId ?? "unknown", currentUserEmail, 
                    "UnauthorizedAuditLogAccess", "AuditLog", null, null, 
                    new { AttemptedUserId = userId });
                return Forbid();
            }

            await _auditLogService.LogAsync(currentUserId ?? "unknown", currentUserEmail, 
                "UserAuditLogsAccessed", "AuditLog", null, null, 
                new { ViewedUserId = userId, Filter = filter });

            var query = _context.AuditLogs
                .Where(l => l.UserId == userId);

            // Apply additional filters
            if (filter.FromDate.HasValue)
                query = query.Where(l => l.Timestamp >= filter.FromDate);

            if (filter.ToDate.HasValue)
                query = query.Where(l => l.Timestamp <= filter.ToDate);

            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(l => l.Action.Contains(filter.Action));

            var totalCount = await query.CountAsync();

            var logs = await query.OrderByDescending(l => l.Timestamp)
                                .Skip((filter.PageNumber - 1) * filter.PageSize)
                                .Take(filter.PageSize)
                                .Select(l => new AuditLogDto
                                {
                                    Id = l.Id,
                                    UserId = l.UserId,
                                    UserEmail = l.UserEmail,
                                    Action = l.Action,
                                    EntityType = l.EntityType,
                                    EntityId = l.EntityId,
                                    OldValues = l.OldValues,
                                    NewValues = l.NewValues,
                                    Timestamp = l.Timestamp,
                                    IpAddress = l.IpAddress
                                })
                                .ToListAsync();

            var response = new AuditLogResponseDto
            {
                Logs = logs,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };

            return Ok(response);
        }

        [HttpGet("actions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActions()
        {
            var actions = await _context.AuditLogs
                .Select(l => l.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            return Ok(actions);
        }

        [HttpGet("entity-types")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEntityTypes()
        {
            var entityTypes = await _context.AuditLogs
                .Where(l => !string.IsNullOrEmpty(l.EntityType))
                .Select(l => l.EntityType)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();

            return Ok(entityTypes);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var currentUserEmail = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? "unknown";
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            
            await _auditLogService.LogAsync(currentUserId, currentUserEmail, "AuditStatsAccessed", 
                "AuditLog", null, null, new { FromDate = fromDate, ToDate = toDate });

            var query = _context.AuditLogs.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(l => l.Timestamp >= fromDate);

            if (toDate.HasValue)
                query = query.Where(l => l.Timestamp <= toDate);

            var stats = new
            {
                TotalLogs = await query.CountAsync(),
                UniqueUsers = await query.Select(l => l.UserId).Distinct().CountAsync(),
                ActionBreakdown = await query.GroupBy(l => l.Action)
                    .Select(g => new { Action = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync(),
                EntityTypeBreakdown = await query.Where(l => !string.IsNullOrEmpty(l.EntityType))
                    .GroupBy(l => l.EntityType)
                    .Select(g => new { EntityType = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync(),
                RecentActivity = await query.OrderByDescending(l => l.Timestamp)
                    .Take(10)
                    .Select(l => new { l.Action, l.UserEmail, l.Timestamp })
                    .ToListAsync()
            };

            return Ok(stats);
        }
    }
}