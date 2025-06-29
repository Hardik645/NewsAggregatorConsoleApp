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
        private readonly List<(string Label, string Value)> _articleFields = new();

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

                var response = await ArticleService.GetArticleById(pageSharedStorage, articleId.Value);
                await ProcessArticleResponse(response);

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
                    await HandleFeedback(articleId.Value, true);
                }
                else if (key == ConsoleKey.D)
                {
                    await HandleFeedback(articleId.Value, false);
                }
                else if (key == ConsoleKey.S && !isSavedArticleView)
                {
                    await HandleSaveArticle(articleId.Value);
                }
                else if (key == ConsoleKey.U && isSavedArticleView)
                {
                    if (await HandleUnsaveArticle(articleId.Value))
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

        private async Task ProcessArticleResponse(ResponseMessage response)
        {
            _articleFields.Clear();
            if (response.StatusCode == HttpStatusCode.OK && response.Data is JsonObject data)
            {
                string title = data["title"]?.ToString() ?? "(No Title)";
                string content = data["content"]?.ToString() ?? "(No Content)";
                string publishedAt = "";
                if (DateTime.TryParse(data["publishedAt"]?.ToString(), out var dt))
                {
                    publishedAt = dt.ToString("g");
                }
                string sourceName = data["source"]?["name"]?.ToString() ?? "(Unknown Source)";
                string categoryName = data["category"]?["name"]?.ToString() ?? "(Unknown Category)";
                string url = data["url"]?.ToString() ?? "(No Image)";
                string likes = data["likes"]?.ToString() ?? "0";
                string dislikes = data["dislikes"]?.ToString() ?? "0";

                _articleFields.Add(("Title", title));
                _articleFields.Add(("Published At", publishedAt));
                _articleFields.Add(("Source", sourceName));
                _articleFields.Add(("Category", categoryName));
                _articleFields.Add(("URL", url));
                _articleFields.Add(("Likes", likes));
                _articleFields.Add(("Dislikes", dislikes));
                _articleFields.Add(("Content", content));
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                await PageHelper.ShowInfoToast("Not found");
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to fetch article");
            }
        }
        private void DisplayFields()
        {
            int labelWidth = PageHelper.ConsoleWidth() > 120 ? 30 : 10;
            int valueWidth = PageHelper.ConsoleWidth() > 120 ? 25 : 15;

            foreach (var (label, value) in _articleFields)
            {
                if (label == "Content")
                {
                    Console.WriteLine();
                    PageHelper.PrintTwoColoredTexts(labelWidth, valueWidth, $"{label}:", value, ConsoleColor.Blue);
                }
                else
                {
                    PageHelper.PrintTwoColoredTexts(labelWidth, valueWidth, $"{label}:", value, ConsoleColor.Blue);
                    Console.WriteLine();
                }
            }
        }
        private async Task HandleFeedback(int articleId, bool isLike)
        {
            var response = await ArticleService.SendArticleFeedback(pageSharedStorage, articleId, isLike);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await PageHelper.ShowSuccessToast(isLike ? "Article liked!" : "Article disliked!");
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to send feedback");
            }
        }
        private async Task HandleSaveArticle(int articleId)
        {
            var response = await ArticleService.SaveArticle(pageSharedStorage, articleId);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await PageHelper.ShowSuccessToast("Article saved!");
            }
            else if(response.StatusCode == HttpStatusCode.Conflict)
            {
                await PageHelper.ShowInfoToast("Article already saved");
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to save article");
            }
        }
        private async Task<bool> HandleUnsaveArticle(int articleId)
        {
            var response = await ArticleService.UnsaveArticle(pageSharedStorage, articleId);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await PageHelper.ShowSuccessToast("Article removed from saved articles!");
                pageSharedStorage.Headlines.RemoveAll(h => h.Id == articleId);
                return true;
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                await PageHelper.ShowInfoToast("Article is not in your saved articles.");
            }
            else
            {
                await PageHelper.ShowErrorToast("Failed to remove article from saved articles.");
            }
            return false;
        }
    }
}