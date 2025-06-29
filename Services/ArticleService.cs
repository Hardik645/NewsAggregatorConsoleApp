using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class ArticleService : ApiService
    {
        public static async Task<ResponseMessage> SaveArticle(PageSharedStorage pageSharedStorage, int articleId)
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
        public static async Task<ResponseMessage> UnsaveArticle(PageSharedStorage pageSharedStorage, int articleId)
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

        public static async Task<ResponseMessage> SendArticleFeedback(PageSharedStorage pageSharedStorage, int articleId, bool isLike)
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

        public static async Task<ResponseMessage> GetArticleById(PageSharedStorage pageSharedStorage, int articleId)
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
    }
}