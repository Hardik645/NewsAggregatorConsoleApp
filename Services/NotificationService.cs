using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class NotificationService:ApiService
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
    }
}
