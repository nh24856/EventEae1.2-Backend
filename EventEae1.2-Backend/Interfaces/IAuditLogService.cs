using System.Threading.Tasks;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(string userId, string userEmail, string action,
                     string entityType = null, string entityId = null,
                     object oldValues = null, object newValues = null);

        Task LogWithContextAsync(string action, string entityType = null, 
            string entityId = null, object oldValues = null, object newValues = null);
    }
}