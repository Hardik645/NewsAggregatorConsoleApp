using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Services;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages.Admin
{
    public class ManageHiddenKeywordsPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            while (true)
            {
                List<string> hiddenKeywords = [];
                var response = await KeywordService.GetHiddenKeywords(pageSharedStorage);
                if (response.StatusCode == HttpStatusCode.OK && response.Data is JsonArray keywordsArray)
                {
                    foreach (var keyword in keywordsArray)
                    {
                        string kw = keyword["keyword"]?.ToString()?? "";
                        if(!String.IsNullOrEmpty(kw))
                            hiddenKeywords.Add(kw);
                    }
                }
                else
                {
                    await PageHelper.ShowErrorToast("Failed to fetch hidden keywords.", 2000);
                    return;
                }

                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("Manage Hidden Keywords");
                Console.WriteLine();

                PageHelper.CenterText("Hidden Keywords:\n", color: ConsoleColor.Blue);
                if (hiddenKeywords.Count == 0)
                {
                    PageHelper.CenterText("(No hidden keywords)", color: ConsoleColor.DarkGray);
                }
                else
                {
                    foreach (var kw in hiddenKeywords)
                    {
                        PageHelper.CenterText($"- {kw}", color: ConsoleColor.Yellow);
                        Console.WriteLine();
                    }
                }

                Console.WriteLine();
                PageHelper.CenterText("A - Add Keyword | R - Remove Keyword | B - Back", color: ConsoleColor.Blue);
                var key = Console.ReadKey(true).Key;
                Console.WriteLine();

                if (key == ConsoleKey.B)
                {
                    break;
                }
                else if (key == ConsoleKey.A)
                {
                    PageHelper.CenterText("Enter keyword to hide:");
                    string? addKeyword = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(addKeyword))
                    {
                        string kw = addKeyword.Trim();
                        var addResponse = await KeywordService.AddHiddenKeyword(pageSharedStorage, kw);
                        if (addResponse.StatusCode == HttpStatusCode.OK)
                            await PageHelper.ShowSuccessToast($"Added '{kw}' to hidden keywords.", 1500);
                        else
                            await PageHelper.ShowErrorToast("Failed to add keyword.", 2000);
                    }
                }
                else if (key == ConsoleKey.R)
                {
                    PageHelper.CenterText("Enter keyword to remove from hidden:");
                    string? removeKeyword = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(removeKeyword))
                    {
                        string kw = removeKeyword.Trim();
                        var removeResponse = await KeywordService.RemoveHiddenKeyword(pageSharedStorage, kw);
                        if (removeResponse.StatusCode == HttpStatusCode.OK)
                            await PageHelper.ShowSuccessToast($"Removed '{kw}' from hidden keywords.", 1500);
                        else
                            await PageHelper.ShowErrorToast("Failed to remove keyword.", 2000);
                    }
                }
            }
        }
    }
}