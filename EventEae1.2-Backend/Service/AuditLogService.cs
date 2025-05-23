using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.Repositories;

namespace EventEae1._2_Backend.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogAsync(Guid? userId, AuditAction action, AuditStatus status, string details = null)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                Status = status,
                Timestamp = DateTime.UtcNow,
                Details = details
            };

            await _auditLogRepository.AddAuditLogAsync(auditLog);
        }
    }
}
