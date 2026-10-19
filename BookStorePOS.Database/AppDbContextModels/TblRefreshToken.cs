using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblRefreshToken
{
    public int RefreshTokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? RevokeAt { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
