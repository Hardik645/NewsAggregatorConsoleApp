using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class NewsService() : ApiService
    {
        public static async Task<ResponseMessage> GetTodayHeadlines(PageSharedStorage pageSharedStorage)
        {
            try
            {
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = "/api/news/today",
                    Method = HttpMethod.Get,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new () { Message = $"Error fetching headlines: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ResponseMessage> GetHeadlinesByDateRange(PageSharedStorage pageSharedStorage, DateOnly startDate, DateOnly endDate)
        {
            try
            {
                string url = $"/api/news/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
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