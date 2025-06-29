using NewsAggregatorConsoleApp.Helper;
namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class NotificationsPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.CenterText("Notifications Page\n");
            // TODO: Implement notifications logic
            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
        }
    }
}