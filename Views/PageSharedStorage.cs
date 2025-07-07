using NewsAggregatorConsoleApp.Models;

namespace NewsAggregatorConsoleApp.Views
{
    public class PageSharedStorage
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
        public int ConsoleWidth { get; }
        public List<(int Id, string Title, string PublishedAt)> Headlines { get; set; } = [];
        public string PaginatedTitle { get; set; } = string.Empty;
        public int? ArticleId { get; set; }

        public PageSharedStorage()
        {
#if DEBUG
            IsAuthenticated = true;
            User = new User()
            {
                UserId = new Guid("C4018D88-BDB6-4BB3-B6C4-2AC3F40E1B18"),
                Role = "Admin",
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1ZjY5YWFhNC1lZTA3LTQ5OWQtYWE0Yi0yOWFhOTYzYTZhNzciLCJlbWFpbCI6ImFkbWluQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3NTE4MzIzNTh9.RInQ5tc9yhhATnnTDLzHfN4NdoK-uwWa2onAB8Qh2RY"
            };
            //User = new User()
            //{
            //    UserId = new Guid("d445e927-8dff-43a6-9480-ffc236ec1495"),
            //    Role = "Admin",
            //    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkNDQ1ZTkyNy04ZGZmLTQzYTYtOTQ4MC1mZmMyMzZlYzE0OTUiLCJlbWFpbCI6ImFkbWluQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3NTA0MjAxODN9.o7P56lzb6lA4_7kSW4T0ywh_LRMmQKnUfpQ14lXEbrg"
            //};
#else
                IsAuthenticated = false;
                User = new User();
#endif
            ConsoleWidth = Console.WindowWidth;
        }
    }

}
