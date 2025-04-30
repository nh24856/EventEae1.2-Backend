using System.Security;

namespace EventEae1._2_Backend.Models
{
    public class UserPermission
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int PermissionId { get; set; }

        public Permission Permission { get; set; }
        public User User { get; set; }
    }
}
