// Dtos/AuditLogResponseDto.cs
namespace EventEae1._2_Backend.Dtos
{
    public class AuditLogResponseDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, List<AuditLogEntryDto>> LogsByDate { get; set; }
    }

    public class AuditLogEntryDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; } // "Login" or "Logout"
        public string Status { get; set; } // "Success" or "Failed"
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
    }
}
