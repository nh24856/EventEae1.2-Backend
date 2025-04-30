using Microsoft.EntityFrameworkCore;
using EventEae1._2_Backend.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security;

namespace EventEae1._2_Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.Role);


            modelBuilder.Entity<User>()
                .Property(u => u.Status);

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.Role, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<UserPermission>()
                .HasKey(up => new { up.UserId, up.PermissionId });

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(up => up.PermissionId);

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(up => up.UserId);

            var permissions = new[]
  {
        new Permission { Id = 1, Name = DefaultPermissions.CanApproveManagers },
        new Permission { Id = 2, Name = DefaultPermissions.CanManageUsers },
        new Permission { Id = 3, Name = DefaultPermissions.CanManageSettings },
        new Permission { Id = 4, Name = DefaultPermissions.CanCreateEvents },
        new Permission { Id = 5, Name = DefaultPermissions.CanViewOwnEvents },
        new Permission { Id = 6, Name = DefaultPermissions.CanViewTicketSales },
        new Permission { Id = 7, Name = DefaultPermissions.CanBrowseEvents },
        new Permission { Id = 8, Name = DefaultPermissions.CanViewOwnTickets },
        new Permission { Id = 9, Name = DefaultPermissions.CanManageProfile }
    };

            modelBuilder.Entity<Permission>().HasData(permissions);

           
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Role = "Admin", PermissionId = 1 },
                new RolePermission { Role = "Admin", PermissionId = 2 },
                new RolePermission { Role = "Admin", PermissionId = 3 },
                new RolePermission { Role = "Admin", PermissionId = 9 } 
            );

            
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Role = "Manager", PermissionId = 4 },
                new RolePermission { Role = "Manager", PermissionId = 5 },
                new RolePermission { Role = "Manager", PermissionId = 6 },
                new RolePermission { Role = "Manager", PermissionId = 9 }
            );

            // Seed RolePermissions for User
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Role = "User", PermissionId = 7 },
                new RolePermission { Role = "User", PermissionId = 8 },
                new RolePermission { Role = "User", PermissionId = 9 }
            );
        }
    }
}
