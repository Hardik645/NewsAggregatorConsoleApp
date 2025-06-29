using NewsAggregatorConsoleApp.Helper;

namespace NewsAggregatorConsoleApp.Views.Pages.User.Common
{
    public class PaginatedHeadlinesPage : IPage
    {
        private const int PageSize = 20;
        private readonly PageSharedStorage _pageSharedStorage;

        public PaginatedHeadlinesPage(PageSharedStorage pageSharedStorage)
        {
            _pageSharedStorage = pageSharedStorage;
        }

        public async Task Render()
        {
            var headlines = _pageSharedStorage.Headlines ?? [];
            var title = _pageSharedStorage.PaginatedTitle ?? "";

            int totalPages = (headlines.Count + PageSize - 1) / PageSize;
            int currentPage = 1;

            while (true)
            {
                Console.Clear();
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader(title);
                Console.WriteLine();

                if (headlines.Count == 0)
                {
                    PageHelper.CenterText("No headlines to display.");
                    Console.WriteLine();
                }
                else
                {
                    int start = (currentPage - 1) * PageSize;
                    int end = Math.Min(start + PageSize, headlines.Count);

                    PageHelper.CenterText(PageHelper.JoinWithSpacing(["Id", "Title", "Published At\n"], PageHelper.ConsoleWidth() - 10), color: ConsoleColor.Blue);
                    PageHelper.DrawLine(max: PageHelper.ConsoleWidth(), lineSymbol: '-');
                    Console.WriteLine();

                    for (int i = start; i < end; i++)
                    {
                        var (id, titleText, publishedAt) = headlines[i];
                        string displayTitle = titleText.Length > PageHelper.ConsoleWidth() - 35 ? titleText[..(PageHelper.ConsoleWidth() - 35)] + "..." : titleText;
                        PageHelper.CenterText(PageHelper.JoinWithSpacing([id.ToString(), displayTitle, $"{publishedAt}\n"], PageHelper.ConsoleWidth() - 10));
                    }

                    Console.WriteLine();
                    PageHelper.CenterText($"Page {currentPage} of {totalPages}", color: ConsoleColor.Blue);
                    Console.WriteLine();
                }

                Console.WriteLine();
                PageHelper.CenterText("N - Next Page | P - Previous Page | Q - Quit",color:ConsoleColor.Blue);
                Console.WriteLine();
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.N && currentPage < totalPages)
                    currentPage++;
                else if (key == ConsoleKey.P && currentPage > 1)
                    currentPage--;
                else if (key == ConsoleKey.Q)
                    break;
                else if (key != ConsoleKey.N && key != ConsoleKey.P && key != ConsoleKey.Q)
                    await PageHelper.ShowErrorToast("Invalid choice. Please try again.");
            }
        }
    }
}