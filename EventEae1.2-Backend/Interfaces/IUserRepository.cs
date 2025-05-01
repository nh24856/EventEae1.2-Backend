using EventEae1._2_Backend.Models;

namespace EventEae1._2_Backend.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task<User?> GetUserWithPermissionsAsync(string email);
        Task<List<string>> GetPermissionsByRoleAsync(string role);
    }
}
