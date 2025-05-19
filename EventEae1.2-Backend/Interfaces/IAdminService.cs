using EventEae1._2_Backend.DTOs;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserDto>> GetAllUsersAsync(ClaimsPrincipal user);
        Task UpdateUserPermissionsAsync(Guid userId, List<string> permissions,
            ClaimsPrincipal currentUser);
        Task SetUserLockStatusAsync(Guid userId, bool isLocked, 
            ClaimsPrincipal currentUser);
        Task<List<PendingManagerDto>> GetPendingManagersAsync(ClaimsPrincipal user);
        Task SetManagerApprovalStatusAsync(Guid managerId, bool isApproved, 
            ClaimsPrincipal currentUser);
    }
}
