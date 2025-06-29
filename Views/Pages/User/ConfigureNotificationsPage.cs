using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Services;
using System.Net;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class ConfigureNotificationsPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            while (true)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("Configure Notifications");
                Console.WriteLine();

                var categoriesResponse = await CategoryService.GetAllCategories(pageSharedStorage);
                if (categoriesResponse.StatusCode != HttpStatusCode.OK || categoriesResponse.Data is not JsonArray categoriesArray)
                {
                    await PageHelper.ShowErrorToast("Failed to fetch categories.", 2000);
                    return;
                }

                var configResponse = await NotificationService.GetNotificationConfig(pageSharedStorage);
                if (configResponse.StatusCode != HttpStatusCode.OK || configResponse.Data is not JsonObject configObj)
                {
                    await PageHelper.ShowErrorToast("Failed to fetch notification config.", 2000);
                    return;
                }

                var enabledSet = configObj["categories"] is JsonArray enabledCategories
                    ? [.. enabledCategories.Select(c => c?.ToString() ?? "")]
                    : new HashSet<string>();

                // Show categories
                PageHelper.CenterText("Categories:\n", color: ConsoleColor.Blue);
                Console.WriteLine();
                foreach (var cat in categoriesArray)
                {
                    string name = cat?["name"]?.ToString() ?? "(unknown)";
                    bool enabled = enabledSet.Contains(name);
                    PageHelper.PrintTwoColoredTexts(PageHelper.ConsoleWidth() / 2 - 7, 5, $"[{(enabled ? "X" : " ")}]", name.ToUpper(), enabled ? ConsoleColor.Green : ConsoleColor.Red);
                    Console.WriteLine();
                }

                // Show keywords
                Console.WriteLine();
                PageHelper.CenterText("Keywords:\n", color: ConsoleColor.Blue);
                List<string> keywordsList = [];
                if (configObj["keywords"] is JsonArray keywordsArray && keywordsArray.Count > 0)
                {
                    foreach (var keyword in keywordsArray)
                    {
                        string kw = keyword?.ToString() ?? "(unknown)";
                        keywordsList.Add(kw);
                        PageHelper.PrintTwoColoredTexts(PageHelper.ConsoleWidth() / 2 - 7, 5, "[*]", kw.ToUpper(), ConsoleColor.Yellow);
                        Console.WriteLine();
                    }
                }
                else
                {
                    PageHelper.CenterText("(No keywords configured)", color: ConsoleColor.DarkGray);
                }

                Console.WriteLine();
                PageHelper.CenterText("C - Toggle Category | A - Add Keyword | R - Remove Keyword | B - Back");
                var key = Console.ReadKey(true).Key;
                Console.WriteLine();

                if (key == ConsoleKey.B)
                {
                    break;
                }
                else if (key == ConsoleKey.C)
                {
                    PageHelper.CenterText("Enter the category name to toggle:");
                    string? input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        string trimmedInput = input.Trim();
                        bool currentlyEnabled = enabledSet.Contains(trimmedInput);
                        bool enable = !currentlyEnabled;
                        var toggleResponse = await NotificationService.ToggleCategory(pageSharedStorage, trimmedInput, enable);
                        if (toggleResponse.StatusCode == HttpStatusCode.OK)
                        {
                            await PageHelper.ShowSuccessToast($"Toggled '{trimmedInput}' notification.", 1500);
                        }
                        else
                        {
                            await PageHelper.ShowErrorToast($"Failed to toggle '{trimmedInput}' notification.", 2000);
                        }
                    }
                }
                else if (key == ConsoleKey.A)
                {
                    PageHelper.CenterText("Enter keyword to add:");
                    string? addKeyword = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(addKeyword))
                    {
                        string kw = addKeyword.Trim();
                        if (!keywordsList.Contains(kw, StringComparer.OrdinalIgnoreCase))
                        {
                            keywordsList.Add(kw);
                            var updateResponse = await NotificationService.UpdateKeywords(pageSharedStorage, keywordsList);
                            if (updateResponse.StatusCode == HttpStatusCode.OK)
                                await PageHelper.ShowSuccessToast($"Added keyword '{kw}'.", 1500);
                            else
                                await PageHelper.ShowErrorToast("Failed to update keywords.", 2000);
                        }
                        else
                        {
                            await PageHelper.ShowInfoToast("Keyword already exists.", 1500);
                        }
                    }
                }
                else if (key == ConsoleKey.R)
                {
                    PageHelper.CenterText("Enter keyword to remove:");
                    string? removeKeyword = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(removeKeyword))
                    {
                        string kw = removeKeyword.Trim();
                        if (keywordsList.RemoveAll(x => x.Equals(kw, StringComparison.OrdinalIgnoreCase)) > 0)
                        {
                            var updateResponse = await NotificationService.UpdateKeywords(pageSharedStorage, keywordsList);
                            if (updateResponse.StatusCode == HttpStatusCode.OK)
                                await PageHelper.ShowSuccessToast($"Removed keyword '{kw}'.", 1500);
                            else
                                await PageHelper.ShowErrorToast("Failed to update keywords.", 2000);
                        }
                        else
                        {
                            await PageHelper.ShowInfoToast("Keyword not found.", 1500);
                        }
                    }
                }
            }
        }
    }
}