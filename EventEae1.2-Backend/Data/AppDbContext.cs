using Microsoft.EntityFrameworkCore;
using EventEae1._2_Backend.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security;
using static System.Runtime.InteropServices.JavaScript.JSType;
using EventEae1._2_Backend.Models;

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
        public DbSet<Event> Events { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }

        public DbSet<TicketSale> TicketSales { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
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

            modelBuilder.Entity<TicketType>()
                .Property(t => t.InitialStock)
                .IsRequired()
                .HasDefaultValue(0);

            modelBuilder.Entity<TicketType>()
                .HasOne(t => t.Event)
                 .WithMany(e => e.TicketTypes)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<TicketSale>()
                .HasOne(ts => ts.TicketType)
                .WithMany(tt => tt.TicketSales)
                .HasForeignKey(ts => ts.TicketTypeId)
                .OnDelete(DeleteBehavior.NoAction);



            modelBuilder.Entity<TicketSale>()
                .HasOne(ts => ts.Buyer)
                .WithMany()
                .HasForeignKey(ts => ts.BuyerId)
                .OnDelete(DeleteBehavior.NoAction); 



            modelBuilder.Entity<TicketSale>()
                .HasOne(ts => ts.Event)
                .WithMany(e => e.TicketSales)
                .HasForeignKey(ts => ts.EventId)
                .OnDelete(DeleteBehavior.NoAction); 
                 

          
            modelBuilder.Entity<TicketSale>()
                .Property(ts => ts.SaleTime)
                .HasDefaultValueSql("GETUTCDATE()");


            // RolePermission configuration
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.Role, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // UserPermission configuration
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

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Action)
                .HasConversion<string>(); // Store as string (default behavior)

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Status)
                .HasConversion<string>(); // Store as string

            // Seed Permissions
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

            // Seed RolePermissions (remove Id field or ensure it's unique)
            modelBuilder.Entity<RolePermission>().HasData(
                // Admin permissions
                new RolePermission { Role = "Admin", PermissionId = 1, Id = 1 },
                new RolePermission { Role = "Admin", PermissionId = 2, Id = 2 },
                new RolePermission { Role = "Admin", PermissionId = 3, Id = 3 },
                new RolePermission { Role = "Admin", PermissionId = 9, Id = 4 },

                // Manager permissions
                new RolePermission { Role = "Manager", PermissionId = 4, Id = 5 },
                new RolePermission { Role = "Manager", PermissionId = 5, Id = 6 },
                new RolePermission { Role = "Manager", PermissionId = 6, Id = 7 },
                new RolePermission { Role = "Manager", PermissionId = 9, Id = 8 },

                // Client permissions
                new RolePermission { Role = "client", PermissionId = 7, Id = 9 },
                new RolePermission { Role = "client", PermissionId = 8, Id = 10 },
                new RolePermission { Role = "client", PermissionId = 9, Id = 11 }
            );

            

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserPermissions)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents) // Fixed: Reference OrganizedEvents
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent organizer deletion if events exist
        }
    }
}
