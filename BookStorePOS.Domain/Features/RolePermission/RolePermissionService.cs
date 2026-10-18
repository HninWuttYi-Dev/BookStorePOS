using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.RolePermission;

namespace BookStorePOS.Domain.Features.RolePermission;

public class RolePermissionService : IRolePermissionService
{
    private readonly AppDbContext _db;
    private readonly ILogger<RolePermissionService> _logger;

    public RolePermissionService(AppDbContext db, ILogger<RolePermissionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    private IQueryable<TblRole> ActiveRoles => _db.TblRoles.Where(r => !r.IsDeleted);

    private IQueryable<TblRole> RolesWithPermissions => ActiveRoles
        .Include(r => r.TblRolePermissions)
        .ThenInclude(rp => rp.Permission);

    public async Task<RoleListResponseModel> GetRolesAsync(RoleListRequestModel? request = null)
    {
        _logger.LogInformation("GetRolesAsync => Fetching roles list");
        try
        {
            var pageNumber = request?.PageNumber ?? 1;
            var pageSize = request?.PageSize ?? 10;

            if (pageNumber < 1 || pageSize < 1 || ((long)pageNumber - 1) * pageSize > int.MaxValue)
            {
                return new RoleListResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.ValidationError,
                    Message = "Page number and page size must be positive and within the supported range."
                };
            }

            var query = RolesWithPermissions.AsNoTracking();

            // Hide Customer system role from staff POS role management
            query = query.Where(r => r.RoleName.ToLower() != "customer");

            if (!string.IsNullOrWhiteSpace(request?.RoleName))
            {
                var roleSearch = request.RoleName.Trim().ToLower();
                query = query.Where(r => r.RoleName.ToLower().Contains(roleSearch));
            }

            var totalCount = await query.CountAsync();

            var roleEntities = await query
                .OrderBy(r => r.RoleName)
                .ThenBy(r => r.RoleId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var roles = roleEntities.Select(Map).ToList();

            return new RoleListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Roles retrieved successfully.",
                Data = roles,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetRolesAsync => Error fetching roles");
            return new RoleListResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = $"Failed to retrieve roles: {ex.Message}"
            };
        }
    }

    public async Task<Result<RoleModel>> GetRoleByIdAsync(int id)
    {
        _logger.LogInformation("GetRoleByIdAsync => Fetching role by id: {RoleId}", id);
        try
        {
            var role = await RolesWithPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null)
            {
                return Result<RoleModel>.NotFound("Role not found.");
            }

            return Result<RoleModel>.Success(Map(role), "Role retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetRoleByIdAsync => Error fetching role {RoleId}", id);
            return Result<RoleModel>.SystemError($"Failed to retrieve role: {ex.Message}");
        }
    }

    public async Task<Result<RoleModel>> CreateRoleAsync(RoleCreateRequestModel request)
    {
        _logger.LogInformation("CreateRoleAsync => Creating role: {RoleName}", request.RoleName);
        try
        {
            var error = await Validate(request.RoleName, request.PermissionIds);
            if (error != null)
            {
                return error.Contains("already exists", StringComparison.OrdinalIgnoreCase)
                    ? Result<RoleModel>.Conflict(error)
                    : Result<RoleModel>.ValidationError(error);
            }

            var role = new TblRole
            {
                RoleName = request.RoleName.Trim(),
                IsDeleted = false,
                CreateAt = DateTime.Now
            };

            foreach (var permissionId in request.PermissionIds.Distinct())
            {
                role.TblRolePermissions.Add(new TblRolePermission
                {
                    PermissionId = permissionId
                });
            }

            _db.TblRoles.Add(role);
            await _db.SaveChangesAsync();

            var createdRole = await RolesWithPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoleId == role.RoleId);

            return Result<RoleModel>.Success(createdRole != null ? Map(createdRole) : null, "Role created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateRoleAsync => Error creating role {RoleName}", request.RoleName);
            return Result<RoleModel>.SystemError($"Failed to create role: {ex.Message}");
        }
    }

    public async Task<Result<RoleModel>> UpdateRoleAsync(RoleUpdateRequestModel request)
    {
        _logger.LogInformation("UpdateRoleAsync => Updating role id: {RoleId}", request.RoleId);
        try
        {
            var role = await RolesWithPermissions
                .FirstOrDefaultAsync(r => r.RoleId == request.RoleId);

            if (role == null)
            {
                return Result<RoleModel>.NotFound("Role not found.");
            }

            var error = await Validate(request.RoleName, request.PermissionIds, request.RoleId);
            if (error != null)
            {
                return error.Contains("already exists", StringComparison.OrdinalIgnoreCase)
                    ? Result<RoleModel>.Conflict(error)
                    : Result<RoleModel>.ValidationError(error);
            }

            if (IsCustomer(role))
            {
                return Result<RoleModel>.ValidationError("The system Customer role cannot be modified via the staff portal.");
            }

            if (IsAdmin(role) && (request.RoleName.Trim() != role.RoleName || await StripsAdminPermissions(role, request.PermissionIds)))
            {
                return Result<RoleModel>.ValidationError("The system Admin role cannot be renamed or have permissions removed.");
            }

            role.RoleName = request.RoleName.Trim();
            role.UpdatedAt = DateTime.Now;

            ReplacePermissions(role, request.PermissionIds);

            await _db.SaveChangesAsync();

            var updatedRole = await RolesWithPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoleId == role.RoleId);

            return Result<RoleModel>.Success(updatedRole != null ? Map(updatedRole) : null, "Role updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateRoleAsync => Error updating role {RoleId}", request.RoleId);
            return Result<RoleModel>.SystemError($"Failed to update role: {ex.Message}");
        }
    }

    public async Task<Result<RoleModel>> DeleteRoleAsync(int id)
    {
        _logger.LogInformation("DeleteRoleAsync => Deleting role id: {RoleId}", id);
        try
        {
            var role = await ActiveRoles.FirstOrDefaultAsync(r => r.RoleId == id);
            if (role == null)
            {
                return Result<RoleModel>.NotFound("Role not found.");
            }

            if (IsAdmin(role) || IsCustomer(role))
            {
                return Result<RoleModel>.ValidationError($"The system {role.RoleName} role cannot be deleted.");
            }

            if (await _db.TblUsers.AnyAsync(u => u.RoleId == id && !u.IsDeleted))
            {
                return Result<RoleModel>.Conflict("Role is assigned to active users.");
            }

            role.IsDeleted = true;
            role.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return Result<RoleModel>.Success(null, "Role deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteRoleAsync => Error deleting role {RoleId}", id);
            return Result<RoleModel>.SystemError($"Failed to delete role: {ex.Message}");
        }
    }

    public async Task<PermissionListResponseModel> GetPermissionsAsync(PermissionListRequestModel? request = null)
    {
        _logger.LogInformation("GetPermissionsAsync => Fetching permissions list");
        try
        {
            var pageNumber = request?.PageNumber ?? 1;
            var pageSize = (request != null && request.PageSize > 0) ? request.PageSize : 100;

            if (pageNumber < 1 || pageSize < 1 || ((long)pageNumber - 1) * pageSize > int.MaxValue)
            {
                return new PermissionListResponseModel
                {
                    IsSuccess = false,
                    Type = EnumResType.ValidationError,
                    Message = "Page number and page size must be positive and within the supported range."
                };
            }

            var query = _db.TblPermissions.AsNoTracking().Where(p => !p.IsDeleted);
            var totalCount = await query.CountAsync();

            var permissions = await query
                .OrderBy(p => p.PermissionName)
                .ThenBy(p => p.PermissionId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PermissionModel
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    Description = p.Description
                })
                .ToListAsync();

            return new PermissionListResponseModel
            {
                IsSuccess = true,
                Type = EnumResType.Success,
                Message = "Permissions retrieved successfully.",
                Data = permissions,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPermissionsAsync => Error fetching permissions");
            return new PermissionListResponseModel
            {
                IsSuccess = false,
                Type = EnumResType.SystemError,
                Message = $"Failed to retrieve permissions: {ex.Message}"
            };
        }
    }

    public async Task<Result<object?>> AssignPermissionsAsync(AssignRolePermissionsRequestModel request)
    {
        _logger.LogInformation("AssignPermissionsAsync => Assigning permissions to role id: {RoleId}", request.RoleId);
        try
        {
            var role = await RolesWithPermissions
                .FirstOrDefaultAsync(r => r.RoleId == request.RoleId);

            if (role == null)
            {
                return Result<object?>.NotFound("Role not found.");
            }

            if (!await ValidPermissions(request.PermissionIds))
            {
                return Result<object?>.ValidationError("One or more permissions are invalid or inactive.");
            }

            if (IsAdmin(role) && await StripsAdminPermissions(role, request.PermissionIds))
            {
                return Result<object?>.ValidationError("The system Admin role cannot have permissions removed.");
            }

            ReplacePermissions(role, request.PermissionIds);
            role.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return Result<object?>.Success(null, "Permissions assigned successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AssignPermissionsAsync => Error assigning permissions to role {RoleId}", request.RoleId);
            return Result<object?>.SystemError($"Failed to assign permissions: {ex.Message}");
        }
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        return await (
            from user in _db.TblUsers
            join role in _db.TblRoles on user.RoleId equals role.RoleId
            join rp in _db.TblRolePermissions on role.RoleId equals rp.RoleId
            join permission in _db.TblPermissions on rp.PermissionId equals permission.PermissionId
            where user.UserId == userId && !user.IsDeleted && !role.IsDeleted && !permission.IsDeleted
            select permission.PermissionName)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync();
    }

    private async Task<string?> Validate(string name, List<int>? ids, int? roleId = null)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 50)
            return "Role name is required and must not exceed 50 characters.";

        var normalized = name.Trim().ToUpperInvariant();

        // Prevent creating or renaming to reserved system names
        if (roleId == null && (normalized == "CUSTOMER" || normalized == "ADMIN"))
        {
            return $"The role name '{name.Trim()}' is reserved by the system.";
        }
        if (roleId != null && normalized == "CUSTOMER")
        {
            return "The role name 'Customer' is reserved by the system.";
        }

        bool nameExists = await _db.TblRoles.AnyAsync(r =>
            r.RoleId != roleId &&
            !r.IsDeleted &&
            r.RoleName.ToUpper() == normalized);

        if (nameExists)
            return "Role name already exists.";

        if (!await ValidPermissions(ids))
            return "One or more permissions are invalid or inactive.";

        return null;
    }

    private async Task<bool> ValidPermissions(List<int>? ids)
    {
        if (ids == null || !ids.Any())
            return true;

        var distinctIds = ids.Distinct().ToList();
        var count = await _db.TblPermissions.CountAsync(p => distinctIds.Contains(p.PermissionId) && !p.IsDeleted);
        return count == distinctIds.Count;
    }

    private async Task<bool> StripsAdminPermissions(TblRole role, List<int>? ids)
    {
        var allActivePermissions = await _db.TblPermissions
            .Where(p => !p.IsDeleted)
            .Select(p => p.PermissionId)
            .ToListAsync();

        var desired = (ids ?? new List<int>()).ToHashSet();
        return allActivePermissions.Any(pId => !desired.Contains(pId));
    }

    private static bool IsAdmin(TblRole role)
    {
        return role.RoleId == 1 || string.Equals(role.RoleName, "Admin", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCustomer(TblRole role)
    {
        return string.Equals(role.RoleName, "Customer", StringComparison.OrdinalIgnoreCase);
    }

    private void ReplacePermissions(TblRole role, List<int>? ids)
    {
        var desired = (ids ?? new List<int>()).Distinct().ToHashSet();

        // Remove unselected permissions
        var toRemove = role.TblRolePermissions.Where(rp => !desired.Contains(rp.PermissionId)).ToList();
        foreach (var rp in toRemove)
        {
            _db.TblRolePermissions.Remove(rp);
        }

        // Add newly selected permissions
        var existingIds = role.TblRolePermissions.Select(rp => rp.PermissionId).ToHashSet();
        foreach (var id in desired)
        {
            if (!existingIds.Contains(id))
            {
                role.TblRolePermissions.Add(new TblRolePermission
                {
                    RoleId = role.RoleId,
                    PermissionId = id
                });
            }
        }
    }

    private static RoleModel Map(TblRole role) => new()
    {
        RoleId = role.RoleId,
        RoleName = role.RoleName,
        Permissions = role.TblRolePermissions
            .Where(rp => rp.Permission != null && !rp.Permission.IsDeleted)
            .Select(rp => new PermissionModel
            {
                PermissionId = rp.PermissionId,
                PermissionName = rp.Permission.PermissionName,
                Description = rp.Permission.Description
            })
            .OrderBy(p => p.PermissionName)
            .ToList()
    };
}
