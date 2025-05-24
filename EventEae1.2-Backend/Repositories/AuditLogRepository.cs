
// Repositories/AuditLogRepository.cs
using EventEae1._2_Backend.Data;
using Microsoft.EntityFrameworkCore;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.Dtos;

namespace EventEae1._2_Backend.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAuditLogAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLogResponseDto>> GetAuditLogsByDateAsync()
        {
            var logs = await _context.AuditLogs
                .Include(al => al.User)
                .Where(al => al.UserId.HasValue) // Exclude anonymous logs
                .OrderBy(al => al.Timestamp)
                .Select(al => new
                {
                    al.UserId,
                    FirstName = al.User != null ? al.User.FirstName : "Unknown",
                    LastName = al.User != null ? al.User.LastName : "Unknown",
                    Email = al.User != null ? al.User.Email : "Unknown",
                    al.Id,
                    al.Action,
                    al.Status,
                    al.Timestamp,
                    al.Details,
                    Date = al.Timestamp.Date
                })
                .ToListAsync();

            // Group by UserId and Date
            var groupedLogs = logs
                .GroupBy(l => new { l.UserId, l.FirstName, l.LastName, l.Email, l.Date })
                .Select(g => new AuditLogResponseDto
                {
                    UserId = g.Key.UserId!.Value,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    Email = g.Key.Email,
                    LogsByDate = new Dictionary<string, List<AuditLogEntryDto>>
                    {
                        {
                            g.Key.Date.ToString("yyyy-MM-dd"),
                            g.Select(l => new AuditLogEntryDto
                            {
                                Id = l.Id,
                                Action = l.Action.ToString(),
                                Status = l.Status.ToString(),
                                Timestamp = l.Timestamp,
                                Details = l.Details
                            }).ToList()
                        }
                    }
                })
                .GroupBy(dto => new { dto.UserId, dto.FirstName, dto.LastName, dto.Email })
                .Select(g => new AuditLogResponseDto
                {
                    UserId = g.Key.UserId,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    Email = g.Key.Email,
                    LogsByDate = g.SelectMany(dto => dto.LogsByDate)
                        .GroupBy(kvp => kvp.Key)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.SelectMany(x => x.Value).ToList())
                })
                .ToList();

            return groupedLogs;
        }
    }
}