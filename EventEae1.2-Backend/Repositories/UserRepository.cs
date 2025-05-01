using System;
using EventEae1._2_Backend.Data;
using EventEae1._2_Backend.Interfaces;
using EventEae1._2_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEae1._2_Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserWithPermissionsAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<string>> GetPermissionsByRoleAsync(string role)
        {
            return await _context.RolePermissions
                .Where(rp => rp.Role == role)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();
        }
    }
}
