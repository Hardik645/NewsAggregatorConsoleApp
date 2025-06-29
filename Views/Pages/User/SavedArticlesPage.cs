using NewsAggregatorConsoleApp.Helper;
namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class SavedArticlesPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.CenterText("Saved Articles Page\n");
            // TODO: Implement saved articles logic
            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
        }
    }
}