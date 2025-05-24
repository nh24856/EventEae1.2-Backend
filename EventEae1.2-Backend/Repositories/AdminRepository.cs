using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventEae1._2_Backend.Data;
using EventEae1._2_Backend.DTOs;
using EventEae1._2_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEae1._2_Backend.Repository
{
    public class AdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.UserPermissions)
                    .ThenInclude(up => up.Permission)
                .ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var u in users)
            {
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.Role == u.Role)
                    .Select(rp => rp.Permission.Name)
                    .ToListAsync();

                var allPermissions = rolePermissions
                    .Concat(u.UserPermissions.Select(up => up.Permission.Name))
                    .Distinct()
                    .ToList();

                userDtos.Add(new UserDto
                {
                    Id = u.Id.ToString(),
                    Email = u.Email,
                    Firstname = u.FirstName,
                    Lastname = u.LastName,
                    Role = u.Role,
                    Organization = u.Organization,
                    Status = u.Status,
                    Permissions = allPermissions
                });
            }

            return userDtos;
        }

        public async Task UpdateUserPermissionsAsync(Guid userId, List<string> permissions)
        {
            var existingPermissions = _context.UserPermissions.Where(up => up.UserId == userId);
            _context.UserPermissions.RemoveRange(existingPermissions);

            var permissionEntities = await _context.Permissions
                .Where(p => permissions.Contains(p.Name))
                .ToListAsync();

            var newUserPermissions = permissionEntities.Select(p => new UserPermission
            {
                UserId = userId,
                PermissionId = p.Id
            });

            await _context.UserPermissions.AddRangeAsync(newUserPermissions);
            await _context.SaveChangesAsync();
        }

        public async Task SetUserLockStatusAsync(Guid userId, bool isLocked)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Status = isLocked ? "locked" : "approved";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<PendingManagerDto>> GetPendingManagersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "manager" && u.Status == "pending")
                .Select(u => new PendingManagerDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    OrganizationName = u.Organization
                })
                .ToListAsync();
        }

        public async Task<(bool success, string Email)> SetManagerApprovalStatusAsync(Guid managerId, bool isApproved)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == managerId && u.Role == "manager");

            if (user != null)
            {
                user.Status = isApproved ? "approved" : "locked";
                await _context.SaveChangesAsync();
                return (true, user.Email);
            }
            return (false, null);
        }
    }
}
