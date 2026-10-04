using System;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Shared.Models.Book;
using Microsoft.Extensions.Logging;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BookController> _logger;
    private readonly IFileStorageService _fileStorageService;

    public BookController(IBookService bookService, ILogger<BookController> logger, IFileStorageService fileStorageService)
    {
        _bookService = bookService;
        _logger = logger;
        _fileStorageService = fileStorageService;
    }


    [HttpGet]
    public async Task<IActionResult> GetBooksAsync([FromQuery] BookListRequestModel requestModel)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBooksAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockBooksAsync()
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetLowStockBooksAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookByIdAsync(int id)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBookByIdAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateBookAsync([FromForm] BookCreateRequestModel requestModel, IFormFile? photo)
    {
        try
        {
            _logger.LogInformation("Create Book Async => Creating book");
        
            if (photo != null && photo.Length > 0)
            {
                var url = await _fileStorageService.UploadImageAsync(photo);
                if (url != null)
                {
                    requestModel.CoverImageUrl = url;
                }
            }

            var response = await _bookService.CreateBookAsync(requestModel);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Create Book Async => Failed to create book {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Create Book Async => Book is created successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateBookAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBookAsync(int id, [FromForm] BookPatchRequestModel requestModel, IFormFile? photo)
    {
        try
        {
            _logger.LogInformation("Update Book Async => Updating book");
            requestModel.BookId = id;
        
            if (photo != null && photo.Length > 0)
            {
                var url = await _fileStorageService.UploadImageAsync(photo);
                if (url != null)
                {
                    requestModel.CoverImageUrl = url;
                }
            }

            var response = await _bookService.UpdateBookAsync(requestModel);
            if (!response.isSuccess)
            {
                _logger.LogWarning("Update Book Async => Failed to update book {Message}", response.Message);
                return BadRequest(response);
            }
            _logger.LogInformation("Update Book Async => Book is updated successfully");
            return Ok(response);
    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateBookAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookAsync(int id)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteBookAsync => Exception occurred");
            return StatusCode(500, new { isSuccess = false, Message = "Internal Server Error: " + ex.Message });
        }
    }
}
