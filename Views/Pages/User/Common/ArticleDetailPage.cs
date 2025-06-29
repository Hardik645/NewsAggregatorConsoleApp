using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using System;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages.User.Common
{
    public class ArticleDetailPage(PageSharedStorage pageSharedStorage) : IPage
    {
        private List<(string Label, string Value)> _articleFields = [];

        public async Task Render()
        {
            int? articleId = pageSharedStorage.ArticleId;
            string? title = pageSharedStorage.PaginatedTitle ?? "";
            bool isSavedArticleView = title.Contains("Saved");

            while (true)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader($"Article Details (Id: {articleId})");
                Console.WriteLine();

                _articleFields = await ArticleService.ProcessArticleByIdResponse(pageSharedStorage, articleId!.Value);

                if (_articleFields.Count == 0)
                {
                    await PageHelper.ShowInfoToast("No article details to display.");
                    Console.WriteLine();
                    return;
                }

                DisplayFields();

                Console.WriteLine();
                if (isSavedArticleView)
                {
                    PageHelper.CenterText("L - Like | D - Dislike | U - Unsave Article | B - Back", color: ConsoleColor.Blue);
                }
                else
                {

                    PageHelper.CenterText("L - Like | D - Dislike | S - Save Article | B - Back", color: ConsoleColor.Blue);
                }
                var key = Console.ReadKey(true).Key;
                Console.WriteLine();

                if (key == ConsoleKey.L)
                {
                    await ArticleService.HandleFeedback(pageSharedStorage, articleId.Value, true);
                }
                else if (key == ConsoleKey.D)
                {
                    await ArticleService.HandleFeedback( pageSharedStorage, articleId.Value, false);
                }
                else if (key == ConsoleKey.S && !isSavedArticleView)
                {
                    await ArticleService.HandleSaveArticle(pageSharedStorage, articleId.Value);
                }
                else if (key == ConsoleKey.U && isSavedArticleView)
                {
                    if (await ArticleService.HandleUnsaveArticle(pageSharedStorage, articleId.Value))
                        break;
                }
                else if (key == ConsoleKey.B)
                {
                    break;
                }
                else
                {
                    await PageHelper.ShowErrorToast("Invalid choice. Please try again.");
                }
            }
        }

        
        private void DisplayFields()
        {
            int labelWidth = PageHelper.ConsoleWidth() > 120 ? 30 : 10;
            int valueWidth = PageHelper.ConsoleWidth() > 120 ? 25 : 15;

            foreach (var (label, value) in _articleFields)
            {
                PageHelper.PrintTwoColoredTexts(labelWidth, valueWidth, $"{label}:", value, ConsoleColor.Blue);
                Console.WriteLine();
            }
        }
        
        
        
    }
}