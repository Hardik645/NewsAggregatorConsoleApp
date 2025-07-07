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

        public static async Task<ResponseMessage> GetAllCategories(PageSharedStorage pageSharedStorage)
        {
            try
            {
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = "/api/categories",
                    Method = HttpMethod.Get,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error fetching categories: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ResponseMessage> ToggleCategoryVisibility(PageSharedStorage pageSharedStorage, string categoryId, bool isHidden)
        {
            try
            {
                string url = $"/api/categories/{categoryId}/visibility?isHidden={isHidden.ToString().ToLower()}";
                ResponseMessage res = await ApiService.SendRequest(() => new ApiRequest
                {
                    Url = url,
                    Method = HttpMethod.Post,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error toggling category visibility: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
    }
}