using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Services;
using System.Net;

namespace NewsAggregatorConsoleApp.Views.Pages.Admin
{
    public class ViewReportedArticlePage(PageSharedStorage pageSharedStorage) : IPage
    {
        private const int PageSize = 20;
        private readonly List<(string ArticleId, string Title, string ReportedCount)> _reportedArticles = [];

        public async Task Render()
        {
            var response = await ArticleService.GetReportedArticles(pageSharedStorage);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _reportedArticles.Clear();
                _reportedArticles.AddRange(ArticleService.ParseReportedArticles(response.Data));
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to fetch reported articles.", 2000);
                return;
            }
            int currentPage = 1;
            int totalPages = (_reportedArticles.Count + PageSize - 1) / PageSize;
            while (true)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("Reported Articles");
                Console.WriteLine();

                if (_reportedArticles.Count == 0)
                {
                    await PageHelper.ShowInfoToast("No reported articles found.");
                    break;
                }
                else
                {
                    int start = (currentPage - 1) * PageSize;
                    int end = Math.Min(start + PageSize, _reportedArticles.Count);

                    PageHelper.CenterText(PageHelper.JoinWithSpacing(["Id", "Title", "Reported Count\n"], PageHelper.ConsoleWidth() - 10), color: ConsoleColor.Blue);
                    PageHelper.DrawLine(max: PageHelper.ConsoleWidth(), lineSymbol: '-');
                    Console.WriteLine();

                    for (int i = start; i < end; i++)
                    {
                        var (id, titleText, reportedCount) = _reportedArticles[i];
                        string displayTitle = titleText.Length > PageHelper.ConsoleWidth() - 45 ? titleText[..(PageHelper.ConsoleWidth() - 45)] + "..." : titleText;
                        PageHelper.CenterText(PageHelper.JoinWithSpacing([id.ToString(), displayTitle, $"{reportedCount}\n"], PageHelper.ConsoleWidth() - 10));
                    }

                    Console.WriteLine();
                    PageHelper.CenterText($"Page {currentPage} of {totalPages}", color: ConsoleColor.Blue);
                    Console.WriteLine();
                }

                Console.WriteLine();
                PageHelper.CenterText("N - Next Page | P - Previous Page | H - Hide Article | B - Back", color: ConsoleColor.Blue);
                var key = Console.ReadKey(true).Key;
                Console.WriteLine();

                if (key == ConsoleKey.N && currentPage < totalPages)
                    currentPage++;
                else if (key == ConsoleKey.P && currentPage > 1)
                    currentPage--;
                else if (key == ConsoleKey.H)
                {
                    PageHelper.CenterText("Enter the Article Id to hide: ");
                    string? input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input.Trim(), out int id))
                    {
                        
                        var hideResponse = await ArticleService.HideArticle(pageSharedStorage, id);
                        if (hideResponse.StatusCode == HttpStatusCode.OK)
                        {
                            await PageHelper.ShowSuccessToast("Article hidden successfully.", 1500);
                            _reportedArticles.RemoveAll(a => a.ArticleId == input.Trim());
                            totalPages = (_reportedArticles.Count + PageSize - 1) / PageSize;
                            if (currentPage > totalPages) currentPage = totalPages;
                        }
                        else
                        {
                            await PageHelper.ShowErrorToast("Failed to hide article.", 2000);
                        }
                    }
                }
                else if (key == ConsoleKey.B)
                    break;
                else
                    await PageHelper.ShowErrorToast("Invalid choice. Please try again.");
            }
        }
    }
}