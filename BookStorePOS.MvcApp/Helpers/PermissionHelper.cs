using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace BookStorePOS.MvcApp.Helpers
{
    public static class PermissionHelper
    {
        /// <summary>
        /// Checks if the current authenticated session has the specified permission.
        /// System Administrator (Admin role) inherently has full access to all features.
        /// </summary>
        public static bool HasPermission(this HttpContext? context, string permission)
        {
            if (context == null || string.IsNullOrWhiteSpace(permission)) return false;

            var role = context.Request.Cookies["pos_role"] ?? "";
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var permissionsCookie = context.Request.Cookies["pos_permissions"] ?? "";
            if (string.IsNullOrWhiteSpace(permissionsCookie))
            {
                return false;
            }

            var permissions = permissionsCookie.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the current session has ANY of the specified permissions.
        /// Useful for conditionally rendering action table headers (e.g. actions column).
        /// </summary>
        public static bool HasAnyPermission(this HttpContext? context, params string[] permissions)
        {
            if (context == null || permissions == null || permissions.Length == 0) return false;

            var role = context.Request.Cookies["pos_role"] ?? "";
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var permissionsCookie = context.Request.Cookies["pos_permissions"] ?? "";
            if (string.IsNullOrWhiteSpace(permissionsCookie))
            {
                return false;
            }

            var userPerms = permissionsCookie.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return permissions.Any(p => userPerms.Contains(p, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the current user has the System Administrator (Admin) role.
        /// </summary>
        public static bool IsAdmin(this HttpContext? context)
        {
            if (context == null) return false;
            var role = context.Request.Cookies["pos_role"] ?? "";
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}

