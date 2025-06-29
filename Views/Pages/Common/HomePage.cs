using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Views.Pages.Admin;
using NewsAggregatorConsoleApp.Views.Pages.User;
using System.Linq;

namespace NewsAggregatorConsoleApp.Views.Pages.Common
{
    public class HomePage(PageSharedStorage pageManager, LoginPage loginPage, SignupPage signupPage, SourcesStatusPage sourcesStatusPage, SourcesDetailPage sourcesDetailPage, SourcesEditPage sourcesEditPage, AddCategoryPage addCategoryPage, HeadlinesPage headlinesPage, SavedArticlesPage savedArticlesPage, SearchPage searchPage, NotificationsPage notificationsPage) : IPage
    {
        private record MenuOption(string Key, string Title, Func<Task> Action);

        public async Task Render()
        {
            bool exit = false;

            while (!exit)
            {
                PageHelper.DisplayHeader();
                PageHelper.CenterText("Welcome to News Aggregator\n");
                Console.WriteLine();

                var menuOptions = pageManager.IsAuthenticated ? pageManager.User.Role == "User" ? GetUserMenu() : GetAdminMenu() : GetGuestMenu();
                if (pageManager.User.Role == "User") DisplayMenu(menuOptions);
                else DisplayMenu(menuOptions, 45, 47);

                Console.WriteLine();
                PageHelper.CenterText("Choose an option: ");
                string choice = Console.ReadLine() ?? "";

                exit = await ProcessSelection(choice, menuOptions);
            }
        }

        private List<MenuOption> GetUserMenu() =>
        [
            new("1", "Headlines", () => headlinesPage.Render()),
            new("2", "Saved Articles", () => savedArticlesPage.Render()),
            new("3", "Search", () => searchPage.Render()),
            new("4", "Notifications", () => notificationsPage.Render()),
            new("5", "Logout", () => Logout()),
            new("6", "Exit", () => Exit())
        ];

        private List<MenuOption> GetAdminMenu() =>
        [
            new("1", "View the list of external servers and status", () => sourcesStatusPage.Render()),
            new("2", "View the external server’s details", () => sourcesDetailPage.Render()),
            new("3", "Update/Edit the external server’s details", () => sourcesEditPage.Render()),
            new("4", "Add new News Category", () => addCategoryPage.Render()),
            new("5", "Logout", () => Logout()),
            new("6", "Exit", () => Exit())
        ];

        private List<MenuOption> GetGuestMenu() =>
        [
            new("1", "Login", () => loginPage.Render()),
            new("2", "Sign Up", () => signupPage.Render()),
            new("3", "Exit", () => Exit())
        ];

        private static void DisplayMenu(List<MenuOption> menuOptions, int withSpace = 16, int totalLength = 20)
        {
            PageHelper.CenterLines([.. menuOptions.Select(option => PageHelper.CenterAlignTwoTexts($"{option.Key}.", option.Title, withSpace, totalLength))]);
        }

        private static async Task<bool> ProcessSelection(string choice, List<MenuOption> menuOptions)
        {
            var selectedOption = menuOptions.FirstOrDefault(option => option.Key == choice);

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
