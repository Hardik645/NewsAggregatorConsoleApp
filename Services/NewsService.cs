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

        public static async Task<ResponseMessage> SearchHeadlines(
            PageSharedStorage pageSharedStorage,
            string query,
            DateOnly? startDate,
            DateOnly? endDate,
            string sortBy)
        {
            try
            {
                var url = $"/api/news/search?query={Uri.EscapeDataString(query)}";
                if (startDate.HasValue)
                    url += $"&startDate={startDate:yyyy-MM-dd}";
                if (endDate.HasValue)
                    url += $"&endDate={endDate:yyyy-MM-dd}";
                if (!string.IsNullOrWhiteSpace(sortBy))
                    url += $"&sortBy={Uri.EscapeDataString(sortBy)}";

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
                return new() { Message = $"Error searching headlines: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
    }
}