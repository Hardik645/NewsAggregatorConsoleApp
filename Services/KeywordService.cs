using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class KeywordService : ApiService
    {
        public static async Task<ResponseMessage> GetHiddenKeywords(PageSharedStorage pageSharedStorage)
        {
            string url = "/api/keywords/hidden";
            return await SendRequest(() => new ApiRequest
            {
                Url = url,
                Method = HttpMethod.Get,
                Token = pageSharedStorage.User.Token
            });
        }

        public static async Task<ResponseMessage> AddHiddenKeyword(PageSharedStorage pageSharedStorage, string keyword)
        {
            string url = "/api/keywords/hidden";
            var body =  keyword;
            return await SendRequest(() => new ApiRequest
            {
                Url = url,
                Method = HttpMethod.Post,
                Body = body,
                Token = pageSharedStorage.User.Token
            });
        }

        public static async Task<ResponseMessage> RemoveHiddenKeyword(PageSharedStorage pageSharedStorage, string keyword)
        {
            string url = "/api/keywords/hidden?keyword="+ keyword;
            return await SendRequest(() => new ApiRequest
            {
                Url = url,
                Method = HttpMethod.Delete,
                Token = pageSharedStorage.User.Token
            });
        }
    }
}