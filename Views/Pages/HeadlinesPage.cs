using NewsAggregatorConsoleApp.Helper;

namespace NewsAggregatorConsoleApp.Views.Pages
{
    public class HeadlinesPage(
        PageSharedStorage pageSharedStorage,
        TodayHeadlinesPage todayHeadlinesPage,
        DateRangeHeadlinesPage dateRangeHeadlinesPage
    ) : IPage
    {
        private record MenuOption(string Key, string Title, Func<Task> Action);

        public async Task Render()
        {
            bool exit = false;
            while (!exit)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("HEADLINES");
                PageHelper.CenterText("Please choose the options below\n");
                Console.WriteLine();

                var menuOptions = GetMenuOptions();
                DisplayMenu(menuOptions);

                Console.WriteLine();
                PageHelper.CenterText("Enter your choice: ");
                string choice = Console.ReadLine() ?? "";

                exit = await ProcessSelection(choice, menuOptions);
            }
        }

        private List<MenuOption> GetMenuOptions() =>
        [
            new("1", "Today", () => todayHeadlinesPage.Render()),
            new("2", "Date range", () => dateRangeHeadlinesPage.Render()),
            new("3", "Back to Home", ()=> BackToHome()),
            new("4", "Logout", () => Logout())
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
                return selectedOption.Title.Equals("Logout", StringComparison.CurrentCultureIgnoreCase)
                    || selectedOption.Title.Equals("Back to Home", StringComparison.CurrentCultureIgnoreCase);
                ;
                
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