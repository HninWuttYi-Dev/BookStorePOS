using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BookStorePOS.Database.AppDbContextModels;
using BookStorePOS.Domain.Features.Book;

namespace BookStorePos.WindowForm;

static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        var mainForm = ServiceProvider.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseSqlServer("Server=.;Database=BookStore;User ID=sa;Password=sasa@123;TrustServerCertificate=True;");
        });
        services.AddScoped<BookService>();
        services.AddTransient<MainForm>();
    }
}