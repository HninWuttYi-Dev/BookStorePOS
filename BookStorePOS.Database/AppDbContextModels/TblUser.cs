using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblUser
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? RoleId { get; set; }

    public virtual TblRole? Role { get; set; }

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();

    public virtual ICollection<TblRefreshToken> TblRefreshTokens { get; set; } = new List<TblRefreshToken>();
}
