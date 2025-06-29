using NewsAggregatorConsoleApp.Helper;
namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class SearchPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.CenterText("Search Page\n");
            // TODO: Implement search logic
            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
        }
    }
}