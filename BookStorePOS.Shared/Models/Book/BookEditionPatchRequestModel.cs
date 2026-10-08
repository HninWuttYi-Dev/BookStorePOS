using System;

namespace BookStorePOS.Shared.Models.Book;

public class BookEditionPatchRequestModel
{
    public int BookEditionId { get; set; }
    public int BookId { get; set; }
    public string EditionName { get; set; } = null!;
    public string? Isbn { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? EditionNote { get; set; }
    public DateTime? PublishDate { get; set; }
    public int? PageCount { get; set; }
    public int ReorderLevel { get; set; }
    public string? CoverImageUrl { get; set; }
}
