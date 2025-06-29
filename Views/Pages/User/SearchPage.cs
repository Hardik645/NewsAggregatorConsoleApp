using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using NewsAggregatorConsoleApp.Views.Pages.User.Common;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class SearchPage(PageSharedStorage pageSharedStorage, PaginatedHeadlinesPage paginatedHeadlinesPage) : IPage
    {
        private readonly List<(int Id, string Title, string PublishedAt)> _headlines = [];

        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.DisplaySubHeader("Search News Articles");
            Console.WriteLine();

            PageHelper.CenterText("Enter search query (or '/exit' to cancel): ");
            var query = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(query) || query.Trim().ToLower() == "/exit")
                return;

            DateOnly? startDate = null, endDate = null;
            while (true)
            {
                PageHelper.CenterText("Enter start date (yyyy-MM-dd) or type 'exit' to cancel: ");
                var input = Console.ReadLine();
                if (input?.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase) == true)
                    return;
                if (string.IsNullOrWhiteSpace(input))
                    break;
                if (DateOnly.TryParse(input, out var sDate))
                {
                    startDate = sDate;
                    break;
                }
                PageHelper.ShowErrorToast("Invalid start date format. Please try again.\n", 1500).Wait();
            }

            while (true)
            {
                PageHelper.CenterText("Enter end date (yyyy-MM-dd) or type 'exit' to cancel: ");
                var input = Console.ReadLine();
                if (input?.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase) == true)
                    return;
                if (string.IsNullOrWhiteSpace(input))
                    break;
                if (DateOnly.TryParse(input, out var eDate))
                {
                    endDate = eDate;
                    break;
                }
                PageHelper.ShowErrorToast("Invalid end date format. Please try again.\n", 1500).Wait();
            }

            string[] sortOptions = ["title", "date"];
            PageHelper.CenterText("Sort by: 1. Title  2. Date (default: Date)");
            var sortInput = Console.ReadLine();
            string sortBy = (sortInput == "1" || sortInput?.ToLower() == "title") ? "title" : "date";

            var response = await NewsService.SearchHeadlines(pageSharedStorage, query, startDate, endDate, sortBy);
            await ProcessHeadlinesResponse(response);

            pageSharedStorage.Headlines = _headlines;
            pageSharedStorage.PaginatedTitle = 
                $"Search Results: \"{query}\" | Sort By: {sortBy} | Start: {(startDate?.ToString("yyyy-MM-dd") ?? "Any")} | End: {(endDate?.ToString("yyyy-MM-dd") ?? "Any")}";
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
                await PageHelper.ShowErrorToast($"Failed to fetch search results: {response.Message}", 3000);
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