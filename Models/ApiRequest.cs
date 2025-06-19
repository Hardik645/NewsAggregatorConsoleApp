namespace NewsAggregatorConsoleApp.Models
{
    public class ApiRequest
    {
        public string Url { get; set; } = null!;
        public object? Body { get; set; }
        public HttpMethod Method { get; set; } = null!;
    }
}
