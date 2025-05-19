using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventEae1._2_Backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync(User);
            return Ok(users);
        }

        [HttpPut("users/{userId}/permissions")]
        public async Task<IActionResult> UpdateUserPermissions(Guid userId, [FromBody] UpdatePermissionsDto dto)
        {
            await _adminService.UpdateUserPermissionsAsync(userId, dto.Permissions, User);
            return Ok(new { message = "Permissions updated." });
        }

        [HttpPut("users/{userId}/lock")]
        public async Task<IActionResult> LockOrUnlockUser(Guid userId, [FromQuery] bool isLocked)
        {
            await _adminService.SetUserLockStatusAsync(userId, isLocked, User);
            return Ok(new { message = $"User has been {(isLocked ? "locked" : "unlocked")}." });
        }

        [HttpGet("pending-managers")]
        public async Task<IActionResult> GetPendingManagers()
        {
            var pending = await _adminService.GetPendingManagersAsync(User);
            return Ok(pending);
        }

        [HttpPut("managers/{managerId}/approval")]
        public async Task<IActionResult> ApproveOrRejectManager(Guid managerId, [FromBody] ManagerApprovalDto dto)
        {
            await _adminService.SetManagerApprovalStatusAsync(managerId, dto.Approved, User);
            return Ok(new { message = $"Manager has been {(dto.Approved ? "approved" : "rejected")}." });
        }
    }
}
