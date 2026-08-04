using System;
using System.Net.Http;
using System.Threading.Tasks;
using BookStorePOS.ConsoleApp.Features;

namespace BookStorePOS.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        // Change the port if your WebApi runs on a different one (e.g. 5201)
        string baseUrl = "http://localhost:5201/api"; 
        
        using HttpClient client = new HttpClient();
        
        var bookFeature = new BookFeature(client, baseUrl);
        var orderFeature = new OrderFeature(client, baseUrl);

    MainMenu:
        Console.WriteLine("\n==================================");
        Console.WriteLine("   Book Store POS - Console App");
        Console.WriteLine("==================================");
        Console.WriteLine("1. Manage Books (Inventory)");
        Console.WriteLine("2. Manage Orders (Checkout)");
        Console.WriteLine("3. Exit Program");
        Console.Write("Choose a section: ");
        
        string? choiceStr = Console.ReadLine();
        if (!int.TryParse(choiceStr, out int choice)) choice = 0;

        switch (choice)
        {
            case 1:
                await bookFeature.RunAsync();
                break;
            case 2:
                await orderFeature.RunAsync();
                break;
            case 3:
                goto Exit;
            default:
                Console.WriteLine("Invalid Choice. Try again.");
                break;
        }

        goto MainMenu;

    Exit:
        Console.WriteLine("Exiting the program. Goodbye!");
    }
}
