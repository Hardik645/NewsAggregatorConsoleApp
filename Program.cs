using Microsoft.Extensions.DependencyInjection;
using NewsAggregatorConsoleApp.Views;
using NewsAggregatorConsoleApp.Views.Pages.Admin;
using NewsAggregatorConsoleApp.Views.Pages.Public;
using NewsAggregatorConsoleApp.Views.Pages.User;
using NewsAggregatorConsoleApp.Views.Pages.User.Common;

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


                services.AddTransient<SourcesStatusPage>();
                services.AddTransient<SourcesDetailPage>();
                services.AddTransient<SourcesEditPage>();
                services.AddTransient<AddCategoryPage>();


                services.AddTransient<HeadlinesPage>();
                services.AddTransient<TodayHeadlinesPage>();
                services.AddTransient<DateRangeHeadlinesPage>();
                services.AddTransient<PaginatedHeadlinesPage>();
                services.AddTransient<ArticleDetailPage>();

                services.AddTransient<SavedArticlesPage>();

                services.AddTransient<SearchPage>();

                services.AddTransient<NotificationsPage>();
                services.AddTransient<ViewNotificationsPage>();
                services.AddTransient<ConfigureNotificationsPage>();

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

