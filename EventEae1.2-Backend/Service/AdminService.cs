using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Repository;

namespace EventEae1._2_Backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly AdminRepository _adminRepository;

        public AdminService(AdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // Fetch all users
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _adminRepository.GetAllUsersAsync();
        }

        
        public async Task UpdateUserPermissionsAsync(Guid userId, List<string> permissions)
        {
            await _adminRepository.UpdateUserPermissionsAsync(userId, permissions);
        }

       
        public async Task SetUserLockStatusAsync(Guid userId, bool isLocked)
        {
            await _adminRepository.SetUserLockStatusAsync(userId, isLocked);
        }

        // Get all pending managers
        public async Task<List<PendingManagerDto>> GetPendingManagersAsync()
        {
            return await _adminRepository.GetPendingManagersAsync();
        }

        // Approve or reject manager
        public async Task SetManagerApprovalStatusAsync(Guid managerId, bool isApproved)
        {
            await _adminRepository.SetManagerApprovalStatusAsync(managerId, isApproved);
        }
    }
}
