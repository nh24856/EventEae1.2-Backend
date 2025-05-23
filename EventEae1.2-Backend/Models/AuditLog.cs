using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventEae1._2_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEae1._2_Backend.Models
{
    // Enums for controlled values
    public enum AuditAction
    {
        Login,
        Logout,
        // Add other actions as needed (e.g., PasswordReset, ProfileUpdate)
    }

    public enum AuditStatus
    {
        Success,
        Failed
    }

    [Index(nameof(UserId), nameof(Timestamp), nameof(Action), IsUnique = false)]
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; } // Removed default value

        [ForeignKey("User")]
        public Guid? UserId { get; set; } // Nullable for anonymous actions

        [Required]
        public AuditAction Action { get; set; } // Use enum instead of string

        [Required]
        public AuditStatus Status { get; set; } // Use enum instead of string

        [Required]
        public DateTime Timestamp { get; set; } // No default value here

        [MaxLength(1000)] // For additional context like error messages
        public string? Details { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}