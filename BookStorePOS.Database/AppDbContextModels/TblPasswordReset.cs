using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblPasswordReset
{
    public int PasswordResetId { get; set; }

    public int UserId { get; set; }

    public string OtpCodeHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
