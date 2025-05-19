// Services/AdminService.cs
using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventEae1._2_Backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly AdminRepository _adminRepository;
        private readonly IAuditLogService _auditLogService;

        public AdminService(AdminRepository adminRepository, IAuditLogService auditLogService)
        {
            _adminRepository = adminRepository;
            _auditLogService = auditLogService;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(ClaimsPrincipal user)
        {
            var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserEmail = user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            await _auditLogService.LogAsync(currentUserId, currentUserEmail, "GetAllUsers");
            return await _adminRepository.GetAllUsersAsync();
        }

        public async Task UpdateUserPermissionsAsync(Guid userId, List<string> permissions, ClaimsPrincipal currentUser)
        {
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserEmail = currentUser.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            // Get old permissions for audit log
            var oldPermissions = await _adminRepository.GetUserPermissionsAsync(userId);

            await _adminRepository.UpdateUserPermissionsAsync(userId, permissions);

            await _auditLogService.LogAsync(
                currentUserId,
                currentUserEmail,
                "UserPermissionsUpdated",
                "User",
                userId.ToString(),
                oldPermissions,
                permissions
            );
        }

        public async Task SetUserLockStatusAsync(Guid userId, bool isLocked, ClaimsPrincipal currentUser)
        {
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserEmail = currentUser.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            var user = await _adminRepository.GetUserByIdAsync(userId);
            var oldStatus = user?.Status;

            await _adminRepository.SetUserLockStatusAsync(userId, isLocked);

            await _auditLogService.LogAsync(
                currentUserId,
                currentUserEmail,
                isLocked ? "UserLocked" : "UserUnlocked",
                "User",
                userId.ToString(),
                new { OldStatus = oldStatus },
                new { NewStatus = isLocked ? "locked" : "approved" }
            );
        }

        public async Task<List<PendingManagerDto>> GetPendingManagersAsync(ClaimsPrincipal user)
        {
            var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserEmail = user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            await _auditLogService.LogAsync(currentUserId, currentUserEmail, "GetPendingManagers");
            return await _adminRepository.GetPendingManagersAsync();
        }

        public async Task SetManagerApprovalStatusAsync(Guid managerId, bool isApproved, ClaimsPrincipal currentUser)
        {
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserEmail = currentUser.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            var manager = await _adminRepository.GetUserByIdAsync(managerId);
            var oldStatus = manager?.Status;

            await _adminRepository.SetManagerApprovalStatusAsync(managerId, isApproved);

            await _auditLogService.LogAsync(
                currentUserId,
                currentUserEmail,
                isApproved ? "ManagerApproved" : "ManagerRejected",
                "User",
                managerId.ToString(),
                new { OldStatus = oldStatus },
                new { NewStatus = isApproved ? "approved" : "rejected" }
            );
        }
    }
}