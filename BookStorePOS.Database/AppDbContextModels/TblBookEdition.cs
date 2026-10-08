using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblBookEdition
{
    public int BookEditionId { get; set; }

    public int BookId { get; set; }

    public int EditionId { get; set; }

    public string? Isbn { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? EditionNote { get; set; }

    public DateTime PublishDate { get; set; }

    public int? PageCount { get; set; }

    public int ReorderLevel { get; set; }

    public string? CoverImageUrl { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TblBook Book { get; set; } = null!;

    public virtual TblEdition Edition { get; set; } = null!;
}
