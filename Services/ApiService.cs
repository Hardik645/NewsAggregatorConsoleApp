using NewsAggregatorConsoleApp.Models;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Services
{
    public class ApiService

    {
        private static readonly HttpClient client = new();
        private static readonly string BaseUrl = "https://localhost:7239";

        public static async Task<ResponseMessage> SendRequest(Func<ApiRequest> requestFactory)
        {
            try
            {
                var request = requestFactory();
                string json = JsonSerializer.Serialize(request.Body);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = request.Method == HttpMethod.Post
                    ? await client.PostAsync(BaseUrl + request.Url, content)
                    : await client.GetAsync(BaseUrl + request.Url);

                string responseBody = await response.Content.ReadAsStringAsync();


                ResponseMessage responseJson = new();
                responseJson!.StatusCode = response.StatusCode;
                try
                {
                    JsonNode? jsonNode = JsonNode.Parse(responseBody);
                    responseJson!.Data = jsonNode;
                    responseJson!.Message = jsonNode?["message"]?.ToString()??"";
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        responseJson!.Message = jsonNode?[3]?[0]?[0]?.ToString() ?? "";
                }
                catch { }

                return responseJson;
            }
            catch (Exception ex)
            {
                return new (){ Message = $"Unexpected error: {ex}", StatusCode=HttpStatusCode.BadRequest};
            }
        }
    }
}
