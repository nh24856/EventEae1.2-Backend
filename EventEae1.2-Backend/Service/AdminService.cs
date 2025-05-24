using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Repository;

namespace EventEae1._2_Backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly AdminRepository _adminRepository;
        private readonly IEmailService _emailService;

        public AdminService(AdminRepository adminRepository, IEmailService emailService)
        {
            _adminRepository = adminRepository;
            _emailService = emailService;
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
            var (success, email) = await _adminRepository.SetManagerApprovalStatusAsync(managerId, isApproved);
            if (!success)
            {
                throw new Exception("Manager not found or invalid role.");
            }

            if (!string.IsNullOrEmpty(email))
            {
                await _emailService.SendManagerApprovalEmailAsync(email, isApproved);
            }
        }
    }
}
