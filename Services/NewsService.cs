using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class NewsService() : ApiService
    {
        public static async Task<ResponseMessage> GetTodayHeadlines(PageSharedStorage pageSharedStorage, string? categoryName)
        {
            try
            {
                string url = "/api/news/today";
                if (!string.IsNullOrWhiteSpace(categoryName))
                    url += $"?category={Uri.EscapeDataString(categoryName)}";
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
                return new() { Message = $"Error fetching headlines: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ResponseMessage> GetHeadlinesByDateRange(PageSharedStorage pageSharedStorage, DateOnly startDate, DateOnly endDate, string? categoryName)
        {
            try
            {
                string url = $"/api/news/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                if (!string.IsNullOrWhiteSpace(categoryName))
                    url += $"&category={Uri.EscapeDataString(categoryName)}";
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
                return new() { Message = $"Error fetching headlines: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
    }
}