// Repositories/IAuditLogRepository.cs
using EventEae1._2_Backend.Models;

namespace EventEae1._2_Backend.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAuditLogAsync(AuditLog auditLog);
    }
}