using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using System.Net;

namespace NewsAggregatorConsoleApp.Views.Pages.Admin
{
    public class AddCategoryPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.CenterText("Add New News Category\n");
            Console.WriteLine();

            PageHelper.CenterText("Enter the new category name: ");
            string? categoryName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                await PageHelper.ShowErrorToast("Category name cannot be empty.", 2000);
                return;
            }

            ResponseMessage response = await CategoryService.AddCategory(pageSharedStorage, categoryName);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                await PageHelper.ShowSuccessToast("Category added successfully.", 2000);
            }
            else
            {
                await PageHelper.ShowErrorToast($"Failed to add category: {response.Message}", 3000);
            }

            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
        }
    }
}