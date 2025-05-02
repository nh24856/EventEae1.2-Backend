using EventEae1._2_Backend.DTOs;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task UpdateUserPermissionsAsync(Guid userId, List<string> permissions);
        Task SetUserLockStatusAsync(Guid userId, bool isLocked);
        Task<List<PendingManagerDto>> GetPendingManagersAsync();
        Task SetManagerApprovalStatusAsync(Guid managerId, bool isApproved);
    }
}
