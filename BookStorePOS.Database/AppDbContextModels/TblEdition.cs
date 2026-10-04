using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblEdition
{
    public int EditionId { get; set; }

    public string EditionName { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<TblBookEdition> TblBookEditions { get; set; } = new List<TblBookEdition>();
}
