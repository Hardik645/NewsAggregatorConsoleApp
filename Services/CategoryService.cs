using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class CategoryService : ApiService
    {
        public static async Task<ResponseMessage> AddCategory(PageSharedStorage pageSharedStorage, string categoryName)
        {
            try
            {
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = "/api/categories",
                    Method = HttpMethod.Post,
                    Token = pageSharedStorage.User.Token,
                    Body = categoryName
                });
                return res;
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Message = $"Error adding category: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
    }
}