using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventEae1._2_Backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
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
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("users/{userId}/permissions")]
        public async Task<IActionResult> UpdateUserPermissions(Guid userId, [FromBody] UpdatePermissionsDto dto)
        {
            await _adminService.UpdateUserPermissionsAsync(userId, dto.Permissions);
            return Ok(new { message = "Permissions updated." });
        }

        [HttpPut("users/{userId}/lock")]
        public async Task<IActionResult> LockOrUnlockUser(Guid userId, [FromQuery] bool isLocked)
        {
            await _adminService.SetUserLockStatusAsync(userId, isLocked);
            return Ok(new { message = $"User has been {(isLocked ? "locked" : "unlocked")}." });
        }

        [HttpGet("pending-managers")]
        public async Task<IActionResult> GetPendingManagers()
        {
            var pending = await _adminService.GetPendingManagersAsync();
            return Ok(pending);
        }

        [HttpPut("managers/{managerId}/approval")]
        public async Task<IActionResult> ApproveOrRejectManager(Guid managerId, [FromBody] ManagerApprovalDto dto)
        {
            await _adminService.SetManagerApprovalStatusAsync(managerId, dto.Approved);
            return Ok(new { message = $"Manager has been {(dto.Approved ? "approved" : "rejected")}." });
        }
    }
}
