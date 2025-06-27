using NewsAggregatorConsoleApp.Helper;

namespace NewsAggregatorConsoleApp.Views.Pages
{
    public class DateRangeHeadlinesPage(PageSharedStorage pageSharedStorage) : IPage
    {
        public async Task Render()
        {
            PageHelper.DisplayHeader();
            PageHelper.CenterText("Headlines by Date Range\n");
            // TODO: Implement date range headlines logic
            Console.WriteLine();
            PageHelper.CenterText("Press any key to return...");
            Console.ReadKey();
        }
    }
}