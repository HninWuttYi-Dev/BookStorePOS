using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BookStorePOS.ConsoleApp.Models;

namespace BookStorePOS.ConsoleApp.Features;

public class OrderFeature
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public OrderFeature(HttpClient client, string baseUrl)
    {
        _client = client;
        _baseUrl = baseUrl + "/order";
    }

    public async Task RunAsync()
    {
    StartOrderMenu:
        Console.WriteLine("\n======== Order Management ========");
        Console.WriteLine("1. View Order History");
        Console.WriteLine("2. View Order Details");
        Console.WriteLine("3. Checkout (Create new order)");
        Console.WriteLine("4. Return to Main Menu");
        Console.Write("Choose an option: ");
        
        string? choiceStr = Console.ReadLine();
        if (!int.TryParse(choiceStr, out int choice)) choice = 0;

        switch (choice)
        {
            case 1:
                await ViewOrdersAsync();
                break;
            case 2:
                await ViewOrderByIdAsync();
                break;
            case 3:
                await CreateOrderAsync();
                break;
            case 4:
                return;
            default:
                Console.WriteLine("Invalid Choice. Try again.");
                break;
        }

        goto StartOrderMenu;
    }

    private async Task ViewOrdersAsync()
    {
        var response = await _client.GetAsync(_baseUrl);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OrderListResponseModel>(content);
            if (result?.Data != null)
            {
                foreach (var order in result.Data)
                {
                    Console.WriteLine($"Order #{order.OrderId} | {order.OrderDate?.ToString("dd/MM/yyyy hh:mm:ss tt")} | Total: ${order.TotalPrice}");
                }
            }
        }
        else
        {
            Console.WriteLine($"Failed to load orders: {response.StatusCode}");
        }
    }

    private async Task ViewOrderByIdAsync()
    {
        Console.Write("Enter Order ID: ");
        string? id = Console.ReadLine();
        var response = await _client.GetAsync($"{_baseUrl}/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OrderResponseModel>(content);
            if (result?.Data != null)
            {
                var order = result.Data;
                Console.WriteLine($"\n=== Order #{order.OrderId} ===");
                Console.WriteLine($"Date: {order.OrderDate?.ToString("dd/MM/yyyy hh:mm:ss tt")}");
                Console.WriteLine($"Total Price: ${order.TotalPrice}");
                Console.WriteLine("Items:");
                foreach(var item in order.Items)
                {
                    Console.WriteLine($"  - {item.BookTitle} x{item.Quantity} (${item.UnitPrice}) = ${item.Subtotal}");
                }
            }
        }
        else
        {
            Console.WriteLine($"Failed to load order: {response.StatusCode}");
        }
    }

    private async Task CreateOrderAsync()
    {
        var requestModel = new OrderCreateRequestModel();
        
        Console.WriteLine("--- Add Books to Cart ---");
        while (true)
        {
            Console.Write("Enter Book ID (or press Enter to finish checkout): ");
            string? bookIdStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(bookIdStr)) break;

            if (int.TryParse(bookIdStr, out int bookId))
            {
                Console.Write("Enter Quantity: ");
                string? qtyStr = Console.ReadLine();
                if (int.TryParse(qtyStr, out int quantity) && quantity > 0)
                {
                    requestModel.Items.Add(new CheckoutItemModel { BookId = bookId, Quantity = quantity });
                    Console.WriteLine("Item added.");
                }
                else
                {
                    Console.WriteLine("Invalid quantity.");
                }
            }
        }

        if (requestModel.Items.Count == 0)
        {
            Console.WriteLine("Checkout cancelled. Cart is empty.");
            return;
        }

        var json = JsonConvert.SerializeObject(requestModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(_baseUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<OrderResponseModel>(responseContent);
            Console.WriteLine("Success: " + result?.Message);
            if (result?.Data != null)
            {
                Console.WriteLine($"Your Order ID is: #{result.Data.OrderId}");
            }
        }
        else
        {
            var result = JsonConvert.DeserializeObject<BaseResponseModel>(responseContent);
            Console.WriteLine("Error: " + result?.Message ?? response.StatusCode.ToString());
        }
    }
}
