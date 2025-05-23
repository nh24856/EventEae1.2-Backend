
// Repositories/AuditLogRepository.cs
using EventEae1._2_Backend.Data;
using Microsoft.EntityFrameworkCore;
using EventEae1._2_Backend.Models;

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
    }
}