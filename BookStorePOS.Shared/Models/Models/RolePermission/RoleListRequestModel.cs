using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.RolePermission;

public class RoleModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<PermissionModel> Permissions { get; set; } = new();
}

public class PermissionModel
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string Module => PermissionName.Contains('.') ? PermissionName.Split('.')[0] : "General";

    public string Action => PermissionName.Contains('.') ? PermissionName.Substring(PermissionName.IndexOf('.') + 1) : PermissionName;

    public string Category => Module switch
    {
        "Book" or "Author" or "Genre" or "Edition" => "Inventory Management",
        "Order" => "Sales & POS",
        "Dashboard" => "Dashboard & Analytics",
        "User" => "Staff & User Management",
        "Role" => "Role & Access Control",
        _ => "General System"
    };

    public string DisplayName => (Module, Action) switch
    {
        ("Book", "View") => "View Books Catalog",
        ("Book", "Create") => "Add New Books",
        ("Book", "Update") => "Edit Books & Stock",
        ("Book", "Delete") => "Delete Books",

        ("Author", "View") => "View Authors List",
        ("Author", "Create") => "Add New Authors",
        ("Author", "Update") => "Edit Authors",
        ("Author", "Delete") => "Delete Authors",

        ("Genre", "View") => "View Genres & Categories",
        ("Genre", "Create") => "Add New Genres",
        ("Genre", "Update") => "Edit Genres",
        ("Genre", "Delete") => "Delete Genres",

        ("Edition", "View") => "View Editions Formats",
        ("Edition", "Create") => "Add New Editions",
        ("Edition", "Update") => "Edit Editions",
        ("Edition", "Delete") => "Delete Editions",

        ("Order", "View") => "View Orders & Receipts",
        ("Order", "Create") => "POS Checkout & New Orders",

        ("Dashboard", "View") => "View Dashboard & Sales Analytics",

        ("User", "View") => "View Staff Members",
        ("User", "Create") => "Add New Staff Accounts",
        ("User", "Update") => "Edit Staff Accounts",
        ("User", "Delete") => "Remove Staff Accounts",

        ("Role", "View") => "View Roles & Permissions",
        ("Role", "Create") => "Create New Roles",
        ("Role", "Update") => "Configure Role Permissions",
        ("Role", "Delete") => "Delete Custom Roles",

        _ => $"{Action} {Module}"
    };

    public string FriendlyDescription => (Module, Action) switch
    {
        ("Book", "View") => "Browse book catalog, prices, and stock inventory",
        ("Book", "Create") => "Register new books with pricing, covers, and initial stock",
        ("Book", "Update") => "Modify book details, prices, and inventory stock counts",
        ("Book", "Delete") => "Permanently remove book listings from the system",

        ("Author", "View") => "Search and browse all author listings",
        ("Author", "Create") => "Register new author profiles in the store catalog",
        ("Author", "Update") => "Update author names and biography details",
        ("Author", "Delete") => "Remove author records from the bookstore",

        ("Genre", "View") => "Browse all registered book categories and genres",
        ("Genre", "Create") => "Create new book categories and genres",
        ("Genre", "Update") => "Rename or update existing genres",
        ("Genre", "Delete") => "Delete unused genres from the catalog",

        ("Edition", "View") => "Browse edition types (Paperback, Hardcover, etc.)",
        ("Edition", "Create") => "Add new book edition format options",
        ("Edition", "Update") => "Modify existing edition format options",
        ("Edition", "Delete") => "Remove edition format options",

        ("Order", "View") => "Browse transaction records, sales history, and invoices",
        ("Order", "Create") => "Use the POS cash register, scan books, and complete customer checkouts",

        ("Dashboard", "View") => "Access revenue charts, daily sales totals, and bestsellers",

        ("User", "View") => "Browse the team list and staff account statuses",
        ("User", "Create") => "Create login accounts for cashiers and store personnel",
        ("User", "Update") => "Update staff credentials, names, and assigned roles",
        ("User", "Delete") => "Deactivate or delete staff accounts",

        ("Role", "View") => "Inspect existing staff roles and access levels",
        ("Role", "Create") => "Define new custom staff roles with granular access",
        ("Role", "Update") => "Adjust permission checkboxes for staff roles",
        ("Role", "Delete") => "Remove custom roles (system Admin is protected)",

        _ => string.IsNullOrWhiteSpace(Description) ? $"Allow {Action} operations on {Module}" : Description
    };

    public string ActionBadgeClass => Action.ToLower() switch
    {
        "create" => "bg-emerald-50 text-emerald-700 border border-emerald-200",
        "view" => "bg-sky-50 text-sky-700 border border-sky-200",
        "update" => "bg-amber-50 text-amber-700 border border-amber-200",
        "delete" => "bg-rose-50 text-rose-700 border border-rose-200",
        _ => "bg-slate-100 text-slate-700 border border-slate-200"
    };
}

public class RoleListRequestModel
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? RoleName { get; set; }
}

public class RoleListResponseModel : Result<List<RoleModel>>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
