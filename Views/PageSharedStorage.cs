using NewsAggregatorConsoleApp.Models;

namespace NewsAggregatorConsoleApp.Views
{
    public class PageSharedStorage
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
        public int ConsoleWidth { get; }

        public PageSharedStorage()
        {
            #if DEBUG1
                IsAuthenticated = true;
                User = new User() { UserId = 20, Email = "test@test.com"};
#else
                IsAuthenticated = false;
                User = new User();
#endif
            ConsoleWidth = Console.WindowWidth;
        }
    }

}
