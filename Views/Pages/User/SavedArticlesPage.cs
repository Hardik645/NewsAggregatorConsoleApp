using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using NewsAggregatorConsoleApp.Views.Pages.User.Common;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class SavedArticlesPage(PageSharedStorage pageSharedStorage, PaginatedHeadlinesPage paginatedHeadlinesPage) : IPage
    {
        private readonly List<(int Id, string Title, string PublishedAt)> _headlines = [];

        public async Task Render()
        {
            var response = await ArticleService.GetSavedArticles(pageSharedStorage);
            await ProcessHeadlinesResponse(response);

            pageSharedStorage.Headlines = _headlines;
            pageSharedStorage.PaginatedTitle = "Saved Articles";
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
                await PageHelper.ShowErrorToast($"Failed to fetch saved articles: {response.Message}", 3000);
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