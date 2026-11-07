using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookStorePOS.Database.AppDbContextModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace BookStorePOS.WebApi.Filters;

public class PermissionAttribute : TypeFilterAttribute
{
    public PermissionAttribute(string permission, bool checkFromDb = false) : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permission, checkFromDb };
    }
}

public class PermissionFilter : IAsyncAuthorizationFilter
{
    private readonly string _permission;
    private readonly bool _checkFromDb;
    private readonly AppDbContext _context;

    public PermissionFilter(string permission, bool checkFromDb, AppDbContext context)
    {
        _permission = permission;
        _checkFromDb = checkFromDb;
        _context = context;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 1. Admin (System Owner) always has full access to all endpoints
        if (context.HttpContext.User.IsInRole("Admin") || 
            context.HttpContext.User.HasClaim(ClaimTypes.Role, "Admin") ||
            context.HttpContext.User.HasClaim("role", "Admin"))
        {
            return;
        }

        bool hasPermission = false;
        var requiredList = _permission.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

        // 2. Customer role: Customers viewing their own orders (Order.View / Order.Create endpoints)
        if (context.HttpContext.User.IsInRole("Customer") || 
            context.HttpContext.User.HasClaim(ClaimTypes.Role, "Customer") ||
            context.HttpContext.User.HasClaim("role", "Customer"))
        {
            if (requiredList.Contains("Order.View", System.StringComparer.OrdinalIgnoreCase) || 
                requiredList.Contains("Order.Create", System.StringComparer.OrdinalIgnoreCase) ||
                requiredList.Contains("Book.View", System.StringComparer.OrdinalIgnoreCase))
            {
                return;
            }
        }

        // 3. Fast check: from JWT claims
        hasPermission = context.HttpContext.User.Claims.Any(c => 
            c.Type == "permission" && requiredList.Contains(c.Value, System.StringComparer.OrdinalIgnoreCase));

        // 4. Dynamic RBAC Database Fallback: if token claim didn't match (e.g. role updated in DB), check live database permissions
        if (!hasPermission)
        {
            var userIdString = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out int userId))
            {
                var user = await _context.TblUsers.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
                if (user != null && user.RoleId.HasValue)
                {
                    hasPermission = await (from rp in _context.TblRolePermissions
                                           join p in _context.TblPermissions on rp.PermissionId equals p.PermissionId
                                           where rp.RoleId == user.RoleId.Value && !p.IsDeleted && requiredList.Contains(p.PermissionName)
                                           select p.PermissionId).AnyAsync();
                }
            }
        }

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}

