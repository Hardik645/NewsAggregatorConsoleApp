using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages
{
    public class TodayHeadlinesPage(PageSharedStorage pageSharedStorage) : IPage
    {
        private readonly List<(int Id, string Title, string PublishedAt)> _headlines = [];

        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.DisplaySubHeader("Today's Headlines");
            Console.WriteLine();

            ResponseMessage response = await NewsService.GetTodayHeadlines(pageSharedStorage);
            await ProcessHeadlinesResponse(response);

            if (_headlines.Count == 0)
            {
                PageHelper.CenterText("No headlines found for today.");
            }
            else
            {
                PageHelper.CenterText(PageHelper.JoinWithSpacing(["Id", "Title", "Published At\n"], PageHelper.ConsoleWidth() - 20), color: ConsoleColor.Blue);
                PageHelper.DrawLine(max: PageHelper.ConsoleWidth(), lineSymbol: '-');
                Console.WriteLine();
                foreach (var (id, title, publishedAt) in _headlines)
                {
                    string displayTitle = title.Length > PageHelper.ConsoleWidth() - 30 ? title[..(PageHelper.ConsoleWidth() - 30)] + "..." : title;
                    PageHelper.CenterText(PageHelper.JoinWithSpacing([id.ToString(), displayTitle, $"{publishedAt}\n"], PageHelper.ConsoleWidth() - 10));
                }
            }

            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
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