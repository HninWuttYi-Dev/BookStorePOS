using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BookStorePOS.Database.AppDbContextModels;

namespace BookStorePOS.WebApi.Data;

public static class AuthDataSeeder
{
    public static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AuthDataSeeder");
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.IPasswordHasher<TblUser>>();

        try
        {
            // 1. Roles
            var roleNames = new[] { "Admin", "Staff", "Customer" };
            var roles = new Dictionary<string, TblRole>();

            foreach (var rName in roleNames)
            {
                var existingRole = await db.TblRoles.FirstOrDefaultAsync(r => r.RoleName == rName && !r.IsDeleted);
                if (existingRole == null)
                {
                    existingRole = new TblRole
                    {
                        RoleName = rName,
                        IsDeleted = false,
                        CreateAt = DateTime.Now
                    };
                    db.TblRoles.Add(existingRole);
                    await db.SaveChangesAsync();
                }
                roles[rName] = existingRole;
            }

            // 2. All Permissions
            var allPermissions = new[]
            {
                "Book.View", "Book.Create", "Book.Update", "Book.Delete",
                "Order.View", "Order.Create",
                "Dashboard.View",
                "User.View", "User.Create", "User.Update", "User.Delete",
                "Genre.View", "Genre.Create", "Genre.Update", "Genre.Delete",
                "Author.View", "Author.Create", "Author.Update", "Author.Delete",
                "Edition.View", "Edition.Create", "Edition.Update", "Edition.Delete",
                "Role.View", "Role.Create", "Role.Update", "Role.Delete"
            };

            var perms = new Dictionary<string, TblPermission>();
            foreach (var pName in allPermissions)
            {
                var existingPerm = await db.TblPermissions.FirstOrDefaultAsync(p => p.PermissionName == pName && !p.IsDeleted);
                if (existingPerm == null)
                {
                    existingPerm = new TblPermission
                    {
                        PermissionName = pName,
                        IsDeleted = false,
                        CreateAt = DateTime.Now
                    };
                    db.TblPermissions.Add(existingPerm);
                    await db.SaveChangesAsync();
                }
                perms[pName] = existingPerm;
            }

            // 3. Assign All Permissions to Admin
            foreach (var p in perms.Values)
            {
                bool hasLink = await db.TblRolePermissions.AnyAsync(rp => rp.RoleId == roles["Admin"].RoleId && rp.PermissionId == p.PermissionId);
                if (!hasLink)
                {
                    db.TblRolePermissions.Add(new TblRolePermission
                    {
                        RoleId = roles["Admin"].RoleId,
                        PermissionId = p.PermissionId
                    });
                }
            }

            // Staff defaults
            var staffPerms = new[]
            {
                "Book.View", "Book.Create", "Book.Update",
                "Order.View", "Order.Create",
                "Dashboard.View",
                "Genre.View", "Author.View", "Edition.View"
            };
            foreach (var pName in staffPerms)
            {
                if (perms.TryGetValue(pName, out var p))
                {
                    bool hasLink = await db.TblRolePermissions.AnyAsync(rp => rp.RoleId == roles["Staff"].RoleId && rp.PermissionId == p.PermissionId);
                    if (!hasLink)
                    {
                        db.TblRolePermissions.Add(new TblRolePermission
                        {
                            RoleId = roles["Staff"].RoleId,
                            PermissionId = p.PermissionId
                        });
                    }
                }
            }

            // Customer defaults
            var customerPerms = new[] { "Book.View", "Order.Create", "Order.View" };
            foreach (var pName in customerPerms)
            {
                if (perms.TryGetValue(pName, out var p))
                {
                    bool hasLink = await db.TblRolePermissions.AnyAsync(rp => rp.RoleId == roles["Customer"].RoleId && rp.PermissionId == p.PermissionId);
                    if (!hasLink)
                    {
                        db.TblRolePermissions.Add(new TblRolePermission
                        {
                            RoleId = roles["Customer"].RoleId,
                            PermissionId = p.PermissionId
                        });
                    }
                }
            }

            await db.SaveChangesAsync();

            // 4. Default Users
            var adminUser = await db.TblUsers.FirstOrDefaultAsync(u => u.Username == "admin" && !u.IsDeleted);
            if (adminUser == null)
            {
                adminUser = new TblUser
                {
                    Username = "admin",
                    FullName = "System Administrator",
                    Phone = "09789456123",
                    Email = "admin@bookstore.com",
                    RoleId = roles["Admin"].RoleId,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                };
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "123");
                db.TblUsers.Add(adminUser);
            }

            var staffUser = await db.TblUsers.FirstOrDefaultAsync(u => u.Username == "staff" && !u.IsDeleted);
            if (staffUser == null)
            {
                staffUser = new TblUser
                {
                    Username = "staff",
                    FullName = "Store Staff",
                    Phone = "09789456124",
                    Email = "staff@bookstore.com",
                    RoleId = roles["Staff"].RoleId,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                };
                staffUser.PasswordHash = passwordHasher.HashPassword(staffUser, "123");
                db.TblUsers.Add(staffUser);
            }

            // Upgrade any legacy plain-text "123" passwords in database to secure hashes
            var plainUsers = await db.TblUsers
                .Where(u => !u.IsDeleted && u.PasswordHash != null && u.PasswordHash == "123")
                .ToListAsync();

            foreach (var user in plainUsers)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "123");
            }

            await db.SaveChangesAsync();
            logger.LogInformation("AuthDataSeeder => Successfully verified and synchronized permissions, roles, and admin links.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "AuthDataSeeder => Warning: Could not auto-seed auth data at startup (DB may not be ready yet): {Message}", ex.Message);
        }
    }
}
