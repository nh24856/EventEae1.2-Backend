using EventEae1._2_Backend.Models;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(Guid? userId, AuditAction action, AuditStatus status, string details = null);
    }
}
