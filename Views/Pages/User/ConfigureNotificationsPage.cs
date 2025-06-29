using NewsAggregatorConsoleApp.Helper;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class ConfigureNotificationsPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.DisplaySubHeader("Configure Notifications");
            Console.WriteLine();
            PageHelper.CenterText("Configuring notifications (not implemented yet).");
            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
        }
    }
}