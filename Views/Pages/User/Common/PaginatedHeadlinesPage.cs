using NewsAggregatorConsoleApp.Helper;

namespace NewsAggregatorConsoleApp.Views.Pages.User.Common
{
    public class PaginatedHeadlinesPage(PageSharedStorage pageSharedStorage, ArticleDetailPage articleDetailPage) : IPage
    {
        private const int PageSize = 20;

        public async Task Render()
        {
            var headlines = pageSharedStorage.Headlines ?? [];
            var title = pageSharedStorage.PaginatedTitle ?? "";

            int totalPages = (headlines.Count + PageSize - 1) / PageSize;
            int currentPage = 1;

            while (true)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader(title);
                Console.WriteLine();

                if (headlines.Count == 0)
                {
                    await PageHelper.ShowInfoToast("No headlines to display.");
                    break;
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
                PageHelper.CenterText("N - Next Page | P - Previous Page | V - View Headline | B - Back", color: ConsoleColor.Blue);
                var key = Console.ReadKey(true).Key;
                Console.WriteLine();

                if (key == ConsoleKey.N && currentPage < totalPages)
                    currentPage++;
                else if (key == ConsoleKey.P && currentPage > 1)
                    currentPage--;
                else if (key == ConsoleKey.B)
                    return;
                else if (key == ConsoleKey.V)
                    await ViewHeadlineById();
                else if (key != ConsoleKey.N && key != ConsoleKey.P && key != ConsoleKey.B)
                    await PageHelper.ShowErrorToast("Invalid choice. Please try again.");
            }
        }

        private async Task ViewHeadlineById()
        {
            Console.WriteLine();
            PageHelper.CenterText("Enter the Id of the headline to view: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out int id))
            {
                await PageHelper.ShowErrorToast("Invalid Id. Please enter a valid number.");
                return;
            }
            pageSharedStorage.ArticleId = id;
            await articleDetailPage.Render();
        }
    }
}