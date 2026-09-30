using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblBook
{
    public int BookId { get; set; }

    public string? Isbn { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int ReorderLevel { get; set; }

    public string? CoverImageUrl { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? AuthorId { get; set; }

    public int? GenreId { get; set; }

    public virtual TblAuthor? AuthorNavigation { get; set; }

    public virtual TblGenre? GenreNavigation { get; set; }

    public virtual ICollection<TblOrderItem> TblOrderItems { get; set; } = new List<TblOrderItem>();
}
