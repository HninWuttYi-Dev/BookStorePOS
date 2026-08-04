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
       return Ok(_bookService.GetBooks(requestModel));
    }

    [HttpGet("{id}")]
    public IActionResult GetBook(int id)
    {
        return Ok(_bookService.GetBook(new BookEditRequestModel{BookId = id}));
    }

    [HttpPost]
    public IActionResult CreateBook([FromBody] BookCreateRequestModel requestModel)
    {
       return Ok(_bookService.CreateBook(requestModel));
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateBook(int id, [FromBody] BookPatchRequestModel requestModel)
    {
        requestModel.BookId = id;
        return Ok(_bookService.UpdateBook(requestModel));
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteBook(int id)
    {
        return Ok(_bookService.DeleteBook(new BookDeleteRequestModel{BookId = id}));
    }
}
