using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Services
{
    public class ArticleService : ApiService
    {
        private static async Task<ResponseMessage> SaveArticle(PageSharedStorage pageSharedStorage, int articleId)
        {
            try
            {
                string url = $"/api/articles/savedArticles?articleId={articleId}";
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Post,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error saving article: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
        private static async Task<ResponseMessage> UnsaveArticle(PageSharedStorage pageSharedStorage, int articleId)
        {
            try
            {
                string url = $"/api/articles/savedArticles?id={articleId}";
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Delete,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error saving article: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
        private static async Task<ResponseMessage> SendArticleFeedback(PageSharedStorage pageSharedStorage, int articleId, bool isLike)
        {
            try
            {
                string url = $"/api/articles/{articleId}/feedback?isLike={isLike.ToString().ToLower()}";
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Post,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error sending feedback: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
        private static async Task<ResponseMessage> GetArticleById(PageSharedStorage pageSharedStorage, int articleId)
        {
            try
            {
                string url = $"/api/articles/{articleId}";
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Get,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error fetching article: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
        public static async Task<ResponseMessage> GetSavedArticles(PageSharedStorage pageSharedStorage)
        {
            try
            {
                string url = "/api/articles/savedArticles";
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Get,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error fetching saved articles: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        public static List<(int Id, string Title, string PublishedAt)> ParseHeadlines(JsonNode? data)
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
        public static async Task HandleSaveArticle(PageSharedStorage pageSharedStorage, int articleId)
        {
            var response = await SaveArticle(pageSharedStorage, articleId);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await PageHelper.ShowSuccessToast("Article saved!");
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                await PageHelper.ShowInfoToast("Article already saved");
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to save article");
            }
        }
        public static async Task HandleFeedback(PageSharedStorage pageSharedStorage, int articleId, bool isLike)
        {
            var response = await ArticleService.SendArticleFeedback(pageSharedStorage, articleId, isLike);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await PageHelper.ShowSuccessToast(isLike ? "Article liked!" : "Article disliked!");
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to send feedback");
            }
        }
        public static async Task<bool> HandleUnsaveArticle(PageSharedStorage pageSharedStorage, int articleId)
        {
            var response = await ArticleService.UnsaveArticle(pageSharedStorage, articleId);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await PageHelper.ShowSuccessToast("Article removed from saved articles!");
                pageSharedStorage.Headlines.RemoveAll(h => h.Id == articleId);
                return true;
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                await PageHelper.ShowInfoToast("Article is not in your saved articles.");
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to remove article from saved articles.");
            }
            return false;
        }
        public static async Task<List<(string Label, string Value)>> ProcessArticleByIdResponse(PageSharedStorage pageSharedStorage, int articleId)
        {
            var response = await GetArticleById(pageSharedStorage, articleId);
            List<(string Label, string Value)> _articleFields = [];
            if (response.StatusCode == HttpStatusCode.OK && response.Data is JsonObject data)
            {
                string title = data["title"]?.ToString() ?? "(No Title)";
                string content = data["content"]?.ToString() ?? "(No Content)";
                string publishedAt = "";
                if (DateTime.TryParse(data["publishedAt"]?.ToString(), out var dt))
                {
                    publishedAt = dt.ToString("g");
                }
                string sourceName = data["source"]?["name"]?.ToString() ?? "(Unknown Source)";
                string categoryName = data["category"]?["name"]?.ToString() ?? "(Unknown Category)";
                string url = data["url"]?.ToString() ?? "(No Image)";
                string likes = data["likes"]?.ToString() ?? "0";
                string dislikes = data["dislikes"]?.ToString() ?? "0";

                _articleFields.Add(("Title", title));
                _articleFields.Add(("Published At", publishedAt));
                _articleFields.Add(("Source", sourceName));
                _articleFields.Add(("Category", categoryName));
                _articleFields.Add(("URL", url));
                _articleFields.Add(("Likes", likes));
                _articleFields.Add(("Dislikes", dislikes));
                _articleFields.Add(("Content", content));
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                await PageHelper.ShowInfoToast("Not found");
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to fetch article");
            }
            return _articleFields;
        }
        
    }
}