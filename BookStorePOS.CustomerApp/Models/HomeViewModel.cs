using System.Collections.Generic;
using BookStorePOS.Shared.Models.Book;

namespace BookStorePOS.CustomerApp.Models;

public class HomeViewModel
{
    public List<BookModel> TrendingBooks { get; set; } = new List<BookModel>();
    public List<BookModel> BestsellerBooks { get; set; } = new List<BookModel>();
}

