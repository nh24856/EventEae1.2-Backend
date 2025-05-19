using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEae1._2_Backend.Models
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(128)]
        public string UserId { get; set; }

        [MaxLength(256)]
        public string UserEmail { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; }

        [MaxLength(50)]
        public string EntityType { get; set; }

        [MaxLength(50)]
        public string EntityId { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string OldValues { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NewValues { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(10)]
        public string HttpMethod { get; set; }

        [MaxLength(500)]
        public string Endpoint { get; set; }

        public int? StatusCode { get; set; }

        [MaxLength(100)]
        public string CorrelationId { get; set; }
    }
}