using System.Collections.Generic;
using System.Threading.Tasks;
using BookStorePOS.Shared.Models.Common;
using BookStorePOS.Shared.Models.RolePermission;

namespace BookStorePOS.Domain.Features.RolePermission;

public interface IRolePermissionService
{
    Task<RoleListResponseModel> GetRolesAsync(RoleListRequestModel? request = null);
    Task<Result<RoleModel>> GetRoleByIdAsync(int id);
    Task<Result<RoleModel>> CreateRoleAsync(RoleCreateRequestModel request);
    Task<Result<RoleModel>> UpdateRoleAsync(RoleUpdateRequestModel request);
    Task<Result<RoleModel>> DeleteRoleAsync(int id);
    Task<PermissionListResponseModel> GetPermissionsAsync(PermissionListRequestModel? request = null);
    Task<Result<object?>> AssignPermissionsAsync(AssignRolePermissionsRequestModel request);
    Task<List<string>> GetUserPermissionsAsync(int userId);
}

