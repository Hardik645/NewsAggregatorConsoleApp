using NewsAggregatorConsoleApp.Helper;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class NotificationsPage(
        PageSharedStorage pageSharedStorage,
        ViewNotificationsPage viewNotificationsPage,
        ConfigureNotificationsPage configureNotificationsPage
    ) : IPage
    {
        public async Task Render()
        {
            bool exit = false;
            var menuOptions = GetMenuOptions();

            while (!exit)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("NOTIFICATIONS");
                Console.WriteLine();

                PageHelper.DisplayMenu(menuOptions, 24, 29);

                Console.WriteLine();
                PageHelper.CenterText("Enter your choice: ");
                var key = Console.ReadKey(true).Key;

                var selectedOption = menuOptions.FirstOrDefault(option => option.Key == key);
                if (selectedOption != null)
                {
                    await selectedOption.Action();
                    if (selectedOption.Title == "Back")
                        exit = true;
                }
                else
                {
                    await PageHelper.ShowErrorToast("Invalid choice. Please try again.");
                }
            }
        }

        private List<PageHelper.MenuOption> GetMenuOptions() =>
        [
            new(ConsoleKey.D1, "View Notifications", async () => await viewNotificationsPage.Render()),
            new(ConsoleKey.D2, "Configure Notifications", async () => await configureNotificationsPage.Render()),
            new(ConsoleKey.B, "Back", () => Task.CompletedTask)
        ];
    }
}