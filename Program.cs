using Microsoft.Extensions.DependencyInjection;
using NewsAggregatorConsoleApp.Views;
using NewsAggregatorConsoleApp.Views.Pages;

namespace NewsAggregatorConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                var services = new ServiceCollection();

                services.AddScoped<PageSharedStorage>();

                services.AddTransient<HomePage>();
                services.AddTransient<LoginPage>();
                services.AddTransient<SignupPage>();


                var serviceProvider = services.BuildServiceProvider();

                var homePage = serviceProvider.GetRequiredService<HomePage>();
                await homePage.Render();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}

