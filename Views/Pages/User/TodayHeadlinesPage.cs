using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using NewsAggregatorConsoleApp.Views.Pages.User.Common;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class TodayHeadlinesPage(PageSharedStorage pageSharedStorage, PaginatedHeadlinesPage paginatedHeadlinesPage) : IPage
    {
        private readonly List<(int Id, string Title, string PublishedAt)> _headlines = [];

        public async Task Render()
        {   
            var categoriesResponse = await CategoryService.GetAllCategories(pageSharedStorage);
            var categories = ParseCategories(categoriesResponse.Data);
            categories.Insert(0, "All");

            string? selectedCategory = await PromptCategorySelection(categories);
            if (selectedCategory == null)
                return;

            ResponseMessage response = await NewsService.GetTodayHeadlines(pageSharedStorage, selectedCategory);
            await ProcessHeadlinesResponse(response);

            pageSharedStorage.Headlines = _headlines;
            pageSharedStorage.PaginatedTitle = selectedCategory == "All" ? "Today's Headlines" : $"Today's Headlines - {selectedCategory.ToUpper()}";
            await paginatedHeadlinesPage.Render();
        }

        private static List<string> ParseCategories(JsonNode? data)
        {
            var result = new List<string>();
            if (data is JsonArray array)
            {
                foreach (var item in array)
                {
                    if (item is JsonObject obj)
                    {
                        string? name = obj["name"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(name) && bool.TryParse(obj["isHidden"]?.ToString(), out bool isHidden) && !isHidden)
                            result.Add(name);
                    }
                }
            }
            return result;
        }

        private static async Task<string?> PromptCategorySelection(List<string> categories)
        {
            while (true)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("Today's Headlines");
                PageHelper.CenterText("Select a category for today's headlines:\n");
                Console.WriteLine();
                for (int i = 0; i < categories.Count; i++)
                {
                    PageHelper.PrintTwoColoredTexts(PageHelper.ConsoleWidth() / 2 - 5, 5, $"{i + 1}.", $"{categories[i].ToUpper()}", ConsoleColor.Blue);
                    Console.WriteLine();
                }
                Console.WriteLine();
                PageHelper.CenterText("Enter the number of your choice (or 'B' to back): ");
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    return null;
                if (input.Trim().Equals("b", StringComparison.CurrentCultureIgnoreCase))
                    return null;
                if (int.TryParse(input, out int idx) && idx >= 1 && idx <= categories.Count)
                    return categories[idx - 1];
                await PageHelper.ShowErrorToast("Invalid selection. Please try again.");
            }
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