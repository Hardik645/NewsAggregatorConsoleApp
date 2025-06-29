using NewsAggregatorConsoleApp.Helper;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class HeadlinesPage(
        PageSharedStorage pageSharedStorage,
        TodayHeadlinesPage todayHeadlinesPage,
        DateRangeHeadlinesPage dateRangeHeadlinesPage
    ) : IPage
    {
        private record MenuOption(ConsoleKey Key, string Title, Func<Task> Action);

        public async Task Render()
        {
            bool exit = false;
            while (!exit)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("HEADLINES");
                Console.WriteLine();
                PageHelper.CenterText("Please choose the options below\n");
                Console.WriteLine();

                var menuOptions = GetMenuOptions();
                DisplayMenu(menuOptions);

                Console.WriteLine();
                PageHelper.CenterText("Press the key for your choice: ");
                var keyInfo = Console.ReadKey(true);
                exit = await ProcessSelection(keyInfo.Key, menuOptions);
            }
        }

        private List<MenuOption> GetMenuOptions() =>
        [
            new(ConsoleKey.D1, "Today", () => todayHeadlinesPage.Render()),
            new(ConsoleKey.D2, "Date range", () => dateRangeHeadlinesPage.Render()),
            new(ConsoleKey.D3, "Back to Home", ()=> BackToHome()),
            new(ConsoleKey.D4, "Logout", () => Logout())
        ];

        private static void DisplayMenu(List<MenuOption> menuOptions, int withSpace = 16, int totalLength = 20)
        {
            PageHelper.CenterLines([.. menuOptions.Select(option => PageHelper.CenterAlignTwoTexts($"{option.Key.ToString().Replace("D", "")}.", option.Title, withSpace, totalLength))]);
        }

        private static async Task<bool> ProcessSelection(ConsoleKey key, List<MenuOption> menuOptions)
        {
            var selectedOption = menuOptions.FirstOrDefault(option => option.Key == key);

            if (selectedOption != null)
            {
                await selectedOption.Action();
                return selectedOption.Title.Equals("Logout", StringComparison.CurrentCultureIgnoreCase)
                    || selectedOption.Title.Equals("Back to Home", StringComparison.CurrentCultureIgnoreCase);
            }

            await ShowInvalidChoiceMessage();
            return false;
        }

        private static async Task ShowInvalidChoiceMessage()
        {
            Console.WriteLine();
            await PageHelper.ShowErrorToast("Invalid choice. Please try again.");
        }
        private async Task Logout()
        {
            Console.WriteLine();
            await PageHelper.ShowInfoToast("Logging out...");
            pageSharedStorage.IsAuthenticated = false;
        }

        private async Task BackToHome()
        {
            Console.WriteLine();
            await PageHelper.ShowInfoToast("Going back to Home...");
        }
    }
}