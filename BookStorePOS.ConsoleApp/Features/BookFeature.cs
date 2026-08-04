using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BookStorePOS.ConsoleApp.Models;

namespace BookStorePOS.ConsoleApp.Features;

public class BookFeature
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public BookFeature(HttpClient client, string baseUrl)
    {
        _client = client;
        _baseUrl = baseUrl + "/book";
    }

    public async Task RunAsync()
    {
    StartBookMenu:
        Console.WriteLine("\n======== Book Management ========");
        Console.WriteLine("1. View all books");
        Console.WriteLine("2. View a specific book by ID");
        Console.WriteLine("3. Add new book");
        Console.WriteLine("4. Update existing book");
        Console.WriteLine("5. Delete book");
        Console.WriteLine("6. Return to Main Menu");
        Console.Write("Choose an option: ");
        
        string? choiceStr = Console.ReadLine();
        if (!int.TryParse(choiceStr, out int choice)) choice = 0;

        switch (choice)
        {
            case 1:
                await ViewAllBooksAsync();
                break;
            case 2:
                await ViewBookByIdAsync();
                break;
            case 3:
                await AddBookAsync();
                break;
            case 4:
                await UpdateBookAsync();
                break;
            case 5:
                await DeleteBookAsync();
                break;
            case 6:
                return;
            default:
                Console.WriteLine("Invalid Choice. Try again.");
                break;
        }

        goto StartBookMenu;
    }

    private async Task ViewAllBooksAsync()
    {
        var response = await _client.GetAsync(_baseUrl);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookListResponseModel>(content);
            if (result?.Data != null)
            {
                foreach (var book in result.Data)
                {
                    Console.WriteLine($"[{book.BookId}] {book.Title} by {book.Author} - ${book.Price} (Stock: {book.StockQuantity})");
                }
            }
        }
        else
        {
            Console.WriteLine($"Failed to load books: {response.StatusCode}");
        }
    }

    private async Task ViewBookByIdAsync()
    {
        Console.Write("Enter Book ID: ");
        string? id = Console.ReadLine();
        var response = await _client.GetAsync($"{_baseUrl}/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookResponseModel>(content);
            if (result?.Data != null)
            {
                var book = result.Data;
                Console.WriteLine($"\nTitle: {book.Title}");
                Console.WriteLine($"Author: {book.Author}");
                Console.WriteLine($"Genre: {book.Genre}");
                Console.WriteLine($"Price: ${book.Price}");
                Console.WriteLine($"Stock: {book.StockQuantity}");
                Console.WriteLine($"Description: {book.Description}");
            }
        }
        else
        {
            Console.WriteLine($"Failed to load book: {response.StatusCode}");
        }
    }

    private async Task AddBookAsync()
    {
        var model = new BookCreateRequestModel();
        
        Console.Write("Title: ");
        model.Title = Console.ReadLine() ?? "";
        Console.Write("Author: ");
        model.Author = Console.ReadLine() ?? "";
        Console.Write("Genre: ");
        model.Genre = Console.ReadLine() ?? "";
        Console.Write("Description: ");
        model.Description = Console.ReadLine();
        
        Console.Write("Price: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal price)) model.Price = price;
        
        Console.Write("Initial Stock: ");
        if (int.TryParse(Console.ReadLine(), out int stock)) model.StockQuantity = stock;

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(_baseUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResponseModel>(responseContent);

        if (response.IsSuccessStatusCode)
            Console.WriteLine("Success: " + result?.Message);
        else
            Console.WriteLine("Error: " + result?.Message ?? response.StatusCode.ToString());
    }

    private async Task UpdateBookAsync()
    {
        Console.Write("Enter Book ID to Update: ");
        string? id = Console.ReadLine();

        var model = new BookPatchRequestModel();
        
        Console.Write("New Title (leave blank to skip): ");
        string? title = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(title)) model.Title = title;
        
        Console.Write("New Author (leave blank to skip): ");
        string? author = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(author)) model.Author = author;

        Console.Write("New Price (leave blank to skip): ");
        string? priceStr = Console.ReadLine();
        if (decimal.TryParse(priceStr, out decimal price)) model.Price = price;
        
        Console.Write("New Stock (leave blank to skip): ");
        string? stockStr = Console.ReadLine();
        if (int.TryParse(stockStr, out int stock)) model.StockQuantity = stock;

        var json = JsonConvert.SerializeObject(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PatchAsync($"{_baseUrl}/{id}", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResponseModel>(responseContent);

        if (response.IsSuccessStatusCode)
            Console.WriteLine("Success: " + result?.Message);
        else
            Console.WriteLine("Error: " + result?.Message ?? response.StatusCode.ToString());
    }

    private async Task DeleteBookAsync()
    {
        Console.Write("Enter Book ID to Delete: ");
        string? id = Console.ReadLine();

        var response = await _client.DeleteAsync($"{_baseUrl}/{id}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResponseModel>(responseContent);

        if (response.IsSuccessStatusCode)
            Console.WriteLine("Success: " + result?.Message);
        else
            Console.WriteLine("Error: " + result?.Message ?? response.StatusCode.ToString());
    }
}
