using System;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Models.Book;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BookController> _logger;

    public BookController(IBookService bookService, ILogger<BookController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }


    [HttpGet]
    public async Task<IActionResult> GetBooksAsync([FromQuery] BookListRequestModel requestModel)
    {
        _logger.LogInformation("Get Books Async => Fetching all books");
        var response = await _bookService.GetBooksAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Books Async => Failed to fetch books {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Books Async => Books fetched successfully");
        return Ok(response);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockBooksAsync()
    {
        _logger.LogInformation("Get Low Stock Books Async => Fetching low stock books");
        var response = await _bookService.GetLowStockBooksAsync();
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Low Stock Books Async => Failed to fetch low stock books {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Low Stock Books Async => Low stock books fetched successfully");
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookByIdAsync(int id)
    {
        _logger.LogInformation("Get Book ById Async => Fetching book by Id");
        var response = await _bookService.GetBookAsync(new BookByIdRequestModel { BookId = id });
        if (!response.isSuccess)
        {
            _logger.LogWarning("Get Book ById Async => Failed to fetch book {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Get Book ById Async => Book fetched successfully");
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBookAsync([FromBody] BookCreateRequestModel requestModel)
    {
        _logger.LogInformation("Create Book Async => Creating book");
        var response = await _bookService.CreateBookAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Create Book Async => Failed to create book {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Create Book Async => Book is created successfully");
        return Ok(response);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] BookPatchRequestModel requestModel)
    {
        _logger.LogInformation("Update Book Async => Updating book");
        requestModel.BookId = id;
        var response = await _bookService.UpdateBookAsync(requestModel);
        if (!response.isSuccess)
        {
            _logger.LogWarning("Update Book Async => Failed to update book {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Update Book Async => Book is updated successfully");
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookAsync(int id)
    {
        _logger.LogInformation("Delete Book Async => Deleting book");
        var response = await _bookService.DeleteBookAsync(
            new BookDeleteRequestModel { BookId = id });
        if (!response.isSuccess)
        {
            _logger.LogWarning("Delete Book Async => Failed to delete book {Message}", response.Message);
            return BadRequest(response);
        }
        _logger.LogInformation("Delete Book Async => Book is deleted successfully");
        return Ok(response);
    }
}
