using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Features.Order;
using BookStorePOS.Domain.Models.Book;
using BookStorePOS.Domain.Models.Order;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using BookStorePOS.Chart.Models;

namespace BookStorePOS.Chart.Controllers;

public class HomeController : Controller
{
    private readonly BookService _bookService;
    private readonly OrderService _orderService;

    public HomeController(BookService bookService, OrderService orderService)
    {
        _bookService = bookService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var bookResponse = await _bookService.GetBooksAsync(new BookListRequestModel());
        var books = bookResponse.isSuccess ? bookResponse.Data : new List<BookModel>();
        
        var orderResponse = await _orderService.GetOrdersAsync(new OrderListRequestModel());
        var orders = orderResponse.isSuccess ? orderResponse.Data : new List<OrderModel>();

        // 1. Total Metrics
        var totalSales = orders.Sum(o => o.TotalPrice);
        var totalOrders = orders.Count;
        var totalStock = books.Sum(b => b.StockQuantity);

        // 2. Sparkline Data
        // Sales over time (group by day)
        var salesHistory = orders
            .Where(o => o.OrderDate.HasValue)
            .GroupBy(o => o.OrderDate.Value.Date)
            .OrderBy(x => x.Key)
            .Select(g => g.Sum(o => o.TotalPrice))
            .ToList();
            
        // Orders over time
        var ordersHistory = orders
            .Where(o => o.OrderDate.HasValue)
            .GroupBy(o => o.OrderDate.Value.Date)
            .OrderBy(x => x.Key)
            .Select(g => (decimal)g.Count())
            .ToList();
            
        // Stock history (we don't have time series for stock, so we'll just plot stock per book for visual variation)
        var stockHistory = books.Select(b => (decimal)b.StockQuantity).ToList();

        // 3. Main Charts Data (Top 5 Books)
        var topBooks = books.OrderByDescending(b => b.StockQuantity).Take(5).ToList();
        List<string> topBookLabels = topBooks.Select(b => b.Title).ToList();
        List<int> topBookStock = topBooks.Select(b => b.StockQuantity).ToList();

        ViewData["TotalSales"] = totalSales.ToString("N0");
        ViewData["TotalOrders"] = totalOrders.ToString("N0");
        ViewData["TotalStock"] = totalStock.ToString("N0");

        ViewData["SalesHistory"] = salesHistory;
        ViewData["OrdersHistory"] = ordersHistory;
        ViewData["StockHistory"] = stockHistory;

        ViewData["TopBookLabels"] = topBookLabels;
        ViewData["TopBookStock"] = topBookStock;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
