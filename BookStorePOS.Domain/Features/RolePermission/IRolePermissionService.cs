using System.Collections.Generic;
using System.Threading.Tasks;
using BookStorePOS.Shared.Models.RolePermission;

namespace BookStorePOS.Domain.Features.RolePermission;

public interface IRolePermissionService
{
    Task<RoleListResponseModel> GetRolesAsync(RoleListRequestModel? request = null);
    Task<RoleByIdResponseModel> GetRoleByIdAsync(int id);
    Task<RoleCreateResponseModel> CreateRoleAsync(RoleCreateRequestModel request);
    Task<RoleUpdateResponseModel> UpdateRoleAsync(RoleUpdateRequestModel request);
    Task<RoleDeleteResponseModel> DeleteRoleAsync(int id);
    Task<PermissionListResponseModel> GetPermissionsAsync(PermissionListRequestModel? request = null);
    Task<AssignRolePermissionsResponseModel> AssignPermissionsAsync(AssignRolePermissionsRequestModel request);
    Task<List<string>> GetUserPermissionsAsync(int userId);
}
