using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Models
{
    public class ResponseMessage
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public JsonNode? Data { get; set; }
    }
}
