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
    public BookController()
    {
        _bookService = new BookService();
    }

    [HttpGet]
    public IActionResult GetBooks([FromQuery] BookListRequestModel requestModel)
    {
        var response = _bookService.GetBooks(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetBook(int id)
    {
        var response = _bookService.GetBook(new BookEditRequestModel{BookId = id});
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPost]
    public IActionResult CreateBook([FromBody] BookCreateRequestModel requestModel)
    {
        var response = _bookService.CreateBook(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateBook(int id, [FromBody] BookPatchRequestModel requestModel)
    {
        requestModel.BookId = id;
        var response = _bookService.UpdateBook(requestModel);
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteBook(int id)
    {
        var response = _bookService.DeleteBook(new BookDeleteRequestModel{BookId = id});
        if (!response.isSuccess) return BadRequest(response);
        return Ok(response);
    }
}
