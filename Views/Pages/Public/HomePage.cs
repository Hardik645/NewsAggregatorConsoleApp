using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Views.Pages.Admin;
using NewsAggregatorConsoleApp.Views.Pages.User;

namespace NewsAggregatorConsoleApp.Views.Pages.Public
{
    public class HomePage(
        PageSharedStorage pageManager,
        LoginPage loginPage,
        SignupPage signupPage,
        SourcesStatusPage sourcesStatusPage,
        SourcesDetailPage sourcesDetailPage,
        SourcesEditPage sourcesEditPage,
        AddCategoryPage addCategoryPage,
        HeadlinesPage headlinesPage,
        SavedArticlesPage savedArticlesPage,
        SearchPage searchPage,
        NotificationsPage notificationsPage
    ) : IPage
    {

        public async Task Render()
        {
            bool exit = false;

            while (!exit)
            {
                PageHelper.DisplayHeader();
                PageHelper.CenterText("Welcome to News Aggregator\n");
                Console.WriteLine();

                var menuOptions = pageManager.IsAuthenticated
                    ? pageManager.User.Role == "User" ? GetUserMenu() : GetAdminMenu()
                    : GetGuestMenu();

                if (pageManager.User.Role == "User")
                    PageHelper.DisplayMenu(menuOptions);
                else
                    PageHelper.DisplayMenu(menuOptions, 45, 47);

                Console.WriteLine();
                PageHelper.CenterText("Press the key for your choice: ");
                var keyInfo = Console.ReadKey(true);
                exit = await ProcessSelection(keyInfo.Key, menuOptions);
            }
        }

        private List<PageHelper.MenuOption> GetUserMenu() =>
        [
            new(ConsoleKey.D1, "Headlines", () => headlinesPage.Render()),
            new(ConsoleKey.D2, "Saved Articles", () => savedArticlesPage.Render()),
            new(ConsoleKey.D3, "Search", () => searchPage.Render()),
            new(ConsoleKey.D4, "Notifications", () => notificationsPage.Render()),
            new(ConsoleKey.D5, "Logout", () => Logout()),
            new(ConsoleKey.D6, "Exit", () => Exit())
        ];

        private List<PageHelper.MenuOption> GetAdminMenu() =>
        [
            new(ConsoleKey.D1, "View the list of external servers and status", () => sourcesStatusPage.Render()),
            new(ConsoleKey.D2, "View the external server’s details", () => sourcesDetailPage.Render()),
            new(ConsoleKey.D3, "Update/Edit the external server’s details", () => sourcesEditPage.Render()),
            new(ConsoleKey.D4, "Add new News Category", () => addCategoryPage.Render()),
            new(ConsoleKey.D5, "Logout", () => Logout()),
            new(ConsoleKey.D6, "Exit", () => Exit())
        ];

        private List<PageHelper.MenuOption> GetGuestMenu() =>
        [
            new(ConsoleKey.D1, "Login", () => loginPage.Render()),
            new(ConsoleKey.D2, "Sign Up", () => signupPage.Render()),
            new(ConsoleKey.D3, "Exit", () => Exit())
        ];

        private static async Task<bool> ProcessSelection(ConsoleKey key, List<PageHelper.MenuOption> menuOptions)
        {
            var selectedOption = menuOptions.FirstOrDefault(option => option.Key == key);

            if (selectedOption != null)
            {
                await selectedOption.Action();
                return selectedOption.Title.Equals("exit", StringComparison.CurrentCultureIgnoreCase);
            }

            await ShowInvalidChoiceMessage();
            return false;
        }

        private async Task Logout()
        {
            Console.WriteLine();
            await PageHelper.ShowInfoToast("Logging out...");
            pageManager.IsAuthenticated = false;
        }

        private static async Task Exit()
        {
            Console.WriteLine();
            await PageHelper.ShowInfoToast("Thank you for using NewsAggregator");
        }

        private static async Task ShowInvalidChoiceMessage()
        {
            Console.WriteLine();
            await PageHelper.ShowErrorToast("Invalid choice. Please try again.");
        }
    }
}
