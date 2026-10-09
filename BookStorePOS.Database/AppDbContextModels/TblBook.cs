using System;
using System.Collections.Generic;

namespace BookStorePOS.Database.AppDbContextModels;

public partial class TblBook
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? AuthorId { get; set; }

    public int? GenreId { get; set; }

    public virtual TblAuthor? Author { get; set; }

    public virtual TblGenre? Genre { get; set; }

    public virtual ICollection<TblBookEdition> TblBookEditions { get; set; } = new List<TblBookEdition>();
}
