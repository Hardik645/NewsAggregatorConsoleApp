using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using NewsAggregatorConsoleApp.Views.Pages.User.Common;
using System.Net;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class SavedArticlesPage(PageSharedStorage pageSharedStorage, PaginatedHeadlinesPage paginatedHeadlinesPage) : IPage
    {
        public async Task Render()
        {
            
            var response = await ArticleService.GetSavedArticles(pageSharedStorage);

            await ProcessSavedArticlesResponse(response);

            await paginatedHeadlinesPage.Render();
        }
        public async Task ProcessSavedArticlesResponse( ResponseMessage response )
        {

            if (response.StatusCode == HttpStatusCode.OK)
            {
                pageSharedStorage.Headlines = ArticleService.ParseHeadlines(response.Data);
                pageSharedStorage.PaginatedTitle = "Saved Articles";
            }
            else
            {
                await PageHelper.ShowErrorToast($"Failed to fetch saved articles: {response.Message}", 3000);
            }
        }
    }
}