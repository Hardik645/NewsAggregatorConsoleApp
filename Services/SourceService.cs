using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Views;
using System.Net;

namespace NewsAggregatorConsoleApp.Services
{
    public class SourceService() : ApiService
    {
        public static async Task<ResponseMessage> GetAllSources(PageSharedStorage pageSharedStorage)
        {
            try
            {
                ResponseMessage res = await SendRequest(() => new ()
                {
                    Url = "/api/admin/source",
                    Method = HttpMethod.Get,
                    Token = pageSharedStorage.User.Token
                });
                return res;
            }
            catch (Exception ex)
            {
                return new() { Message = $"Error during login: {ex.Message}", StatusCode=HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ResponseMessage> UpdateSource(PageSharedStorage pageSharedStorage, int sourceId, object updateRequest)
        {
            try
            {
                ResponseMessage res = await SendRequest(() => new ApiRequest
                {
                    Url = $"/api/admin/source/{sourceId}",
                    Method = HttpMethod.Patch,
                    Token = pageSharedStorage.User.Token,
                    Body = updateRequest
                });
                return res;
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Message = $"Error updating source: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
        }
    }
}
