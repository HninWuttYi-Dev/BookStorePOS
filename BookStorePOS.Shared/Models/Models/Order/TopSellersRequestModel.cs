using System.Collections.Generic;
using BookStorePOS.Shared.Models.Common;

namespace BookStorePOS.Shared.Models.Order;

public class TopSellersRequestModel
{
    public int Limit { get; set; } = 5;
    public string Period { get; set; } = "7days";
}

public class TopSellersResponseModel : Result<List<TopSellerBookModel>>
{
}

public class DashboardTopSellersResponseModel : Result<DashboardTopSellersDataModel>
{
}

public class DashboardTopSellersDataModel
{
    public List<TopSellerBookModel> Today { get; set; } = new();
    public List<TopSellerBookModel> SevenDays { get; set; } = new();
    public List<TopSellerBookModel> ThirtyDays { get; set; } = new();
}

public class TopSellerBookModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? CoverImageUrl { get; set; }
    public int TotalSold { get; set; }
}


