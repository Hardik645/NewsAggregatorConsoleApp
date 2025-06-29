using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using NewsAggregatorConsoleApp.Views.Pages.User.Common;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class DateRangeHeadlinesPage(PageSharedStorage pageSharedStorage, PaginatedHeadlinesPage paginatedHeadlinesPage) : IPage
    {
        private readonly List<(int Id, string Title, string PublishedAt)> _headlines = [];

        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.DisplaySubHeader("Headlines by Date Range");
            Console.WriteLine();

            DateOnly startDate, endDate;
            while (true)
            {
                PageHelper.CenterText("Enter start date (yyyy-MM-dd) or type 'exit' to cancel: ");
                var input = Console.ReadLine();
                if (input?.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase) == true)
                    return;
                if (DateOnly.TryParse(input, out startDate))
                    break;
                PageHelper.ShowErrorToast("Invalid start date format. Please try again.\n", 1500).Wait();
            }
            while (true)
            {
                PageHelper.CenterText("Enter end date (yyyy-MM-dd) or type 'exit' to cancel: ");
                var input = Console.ReadLine();
                if (input?.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase) == true)
                    return;
                if (DateOnly.TryParse(input, out endDate) && endDate >= startDate)
                    break;
                PageHelper.ShowErrorToast("Invalid end date or end date before start date. Please try again.\n", 1500).Wait();
            }

            ResponseMessage response = await NewsService.GetHeadlinesByDateRange(pageSharedStorage, startDate, endDate);
            await ProcessHeadlinesResponse(response);

            pageSharedStorage.Headlines = _headlines;
            pageSharedStorage.PaginatedTitle = $"Headlines by Date Range ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})";
            await paginatedHeadlinesPage.Render();            
        }

        private async Task ProcessHeadlinesResponse(ResponseMessage response)
        {
            _headlines.Clear();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _headlines.AddRange(ParseHeadlines(response.Data));
            }
            else
            {
                await PageHelper.ShowErrorToast($"Failed to fetch headlines: {response.Message}", 3000);
            }
        }

        private static List<(int Id, string Title, string PublishedAt)> ParseHeadlines(JsonNode? data)
        {
            var result = new List<(int Id, string Title, string PublishedAt)>();
            if (data is JsonArray array)
            {
                foreach (var item in array)
                {
                    if (item is JsonObject obj)
                    {
                        int id = obj["id"]?.GetValue<int>() ?? 0;
                        string title = obj["title"]?.ToString() ?? "(No Title)";
                        string publishedAt = "";
                        if (DateTime.TryParse(obj["publishedAt"]?.ToString(), out var dt))
                        {
                            publishedAt = dt.ToString("g");
                        }
                        result.Add((id, title, publishedAt));
                    }
                }
            }
            return result;
        }
    }
}