using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Models.Book;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BookStorePOS.Chart.Controllers;

public class ApexChartsController : Controller
{
    private readonly BookService _bookService;

    public ApexChartsController(BookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var bookResponse = await _bookService.GetBooksAsync(new BookListRequestModel());
        var books = bookResponse.isSuccess ? bookResponse.Data : new List<BookModel>();
        
        List<int> series = books.Select(x => x.StockQuantity).ToList();
        List<string> labels = books.Select(x => x.Title).ToList();
        
        ViewData["Series"] = series;
        ViewData["Labels"] = labels;

        return View();
    }
}
