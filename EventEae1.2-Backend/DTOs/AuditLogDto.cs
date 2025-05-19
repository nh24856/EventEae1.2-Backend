using System;

namespace EventEae1._2_Backend.DTOs
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
    }

    public class AuditLogFilterDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class AuditLogResponseDto
    {
        public List<AuditLogDto> Logs { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}