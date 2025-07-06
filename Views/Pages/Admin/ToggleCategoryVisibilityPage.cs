using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Services;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages.Admin
{
    public class ToggleCategoryVisibilityPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            while (true)
            {
                var categoriesResponse = await CategoryService.GetAllCategories(pageSharedStorage);
                if (categoriesResponse.StatusCode != HttpStatusCode.OK || categoriesResponse.Data is not JsonArray categoriesArray)
                {
                    await PageHelper.ShowErrorToast("Failed to fetch categories.", 2000);
                    return;
                }

                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("Toggle Category Visibility");
                Console.WriteLine();

                foreach (var cat in categoriesArray)
                {
                    string id = cat?["id"]?.ToString() ?? "-";
                    string name = cat?["name"]?.ToString() ?? "(unknown)";
                    bool isHidden = cat?["isHidden"]?.GetValue<bool>() ?? false;
                    PageHelper.PrintTwoColoredTexts(PageHelper.ConsoleWidth() / 2-7, 5, $"[{(isHidden ? "X" : " ")}]", $"{id,2}. {name.ToUpper()}", !isHidden ? ConsoleColor.Green : ConsoleColor.Red);
                    Console.WriteLine();
                }

                Console.WriteLine();
                PageHelper.CenterText("T - Toggle Category Visibility | B - Back", color: ConsoleColor.Blue);

                var key = Console.ReadKey(true).Key;
                Console.WriteLine();

                if (key == ConsoleKey.B)
                    break;
                else if (key == ConsoleKey.T)
                {
                    PageHelper.CenterText("Enter category ID to toggle visibility:");
                    string? input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    string categoryId = input.Trim();
                    var selectedCat = categoriesArray.FirstOrDefault(cat => cat?["id"]?.ToString() == categoryId);
                    if (selectedCat == null)
                    {
                        await PageHelper.ShowErrorToast("Invalid category ID.", 1500);
                        continue;
                    }

                    bool currentHidden = selectedCat["isHidden"]?.GetValue<bool>() ?? false;
                    bool newHidden = !currentHidden;

                    var toggleResponse = await CategoryService.ToggleCategoryVisibility(pageSharedStorage, categoryId, newHidden);
                    if (toggleResponse.StatusCode == HttpStatusCode.OK)
                    {
                        await PageHelper.ShowSuccessToast($"Category visibility updated.", 1200);
                    }
                    else
                    {
                        await PageHelper.ShowErrorToast("Failed to update category visibility.", 2000);
                    }
                }
                else
                {
                    await PageHelper.ShowInfoToast("Invalid choice. Please try again.", 1200);
                }
            }
        }
    }
}