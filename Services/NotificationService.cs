using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class NotificationService : ApiService
    {
        public static async Task<ResponseMessage> GetNotifications(PageSharedStorage pageSharedStorage)
        {
            try
            {
                string url = "/api/notifications";
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
                return new() { Message = $"Error fetching notifications: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ResponseMessage> GetNotificationConfig(PageSharedStorage pageSharedStorage)
        {
            try
            {
                string url = "/api/notifications/config";
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
                return new() { Message = $"Error fetching notification config: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ResponseMessage> ToggleCategory(PageSharedStorage pageSharedStorage, string categoryName, bool enable)
        {
            try
            {
                string url = "/api/notifications/config/category";
                var body = new
                {
                    category = categoryName,
                    enabled = enable
                };
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Put,
                    Body = body,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error toggling category: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ResponseMessage> UpdateKeywords(PageSharedStorage pageSharedStorage, List<string> keywords)
        {
            try
            {
                string url = "/api/notifications/config/keywords";
                var body = new { keywords = keywords };
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Put,
                    Body = body,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error updating keywords: {ex.Message}", StatusCode = System.Net.HttpStatusCode.BadRequest };
            }
        }
    }
}
