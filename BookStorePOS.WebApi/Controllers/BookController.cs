using System;
using Microsoft.AspNetCore.Mvc;
using BookStorePOS.Domain.Features.Book;
using BookStorePOS.Domain.Models.Book;

namespace BookStorePOS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly BookService _bookService;

    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }


    [HttpGet]
    public async Task<IActionResult> GetBooksAsync([FromQuery] BookListRequestModel requestModel)
    {
        var response = await _bookService.GetBooksAsync(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookByIdAsync(int id)
    {
        var response = await _bookService.GetBookAsync(new BookByIdRequestModel { BookId = id });
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBookAsync([FromBody] BookCreateRequestModel requestModel)
    {
        var response = await _bookService.CreateBookAsync(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] BookPatchRequestModel requestModel)
    {
        requestModel.BookId = id;
        var response = await _bookService.UpdateBookAsync(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBookAsync(int id)
    {
        var response = await _bookService.DeleteBookAsync(
            new BookDeleteRequestModel { BookId = id });
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }
}
