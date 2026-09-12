using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Models.Book;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BookStorePOS.Chart.Controllers;

public class CanvasJsController : Controller
{
    private readonly BookService _bookService;

    public CanvasJsController(BookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var bookResponse = await _bookService.GetBooksAsync(new BookListRequestModel());
        var books = bookResponse.isSuccess ? bookResponse.Data : new List<BookModel>();
        
        var dataPoints = books.Select(x => new { label = x.Title, y = x.Price }).ToList();
        
        ViewData["DataPoints"] = dataPoints;

        return View();
    }
}
