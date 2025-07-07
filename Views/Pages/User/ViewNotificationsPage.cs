using NewsAggregatorConsoleApp.Helper;
using NewsAggregatorConsoleApp.Models;
using NewsAggregatorConsoleApp.Services;
using NewsAggregatorConsoleApp.Views.Pages.User.Common;
using System.Net;
using System.Text.Json.Nodes;

namespace NewsAggregatorConsoleApp.Views.Pages.User
{
    public class ViewNotificationsPage(PageSharedStorage pageSharedStorage, ArticleDetailPage articleDetailPage) : IPage
    {
        private const int PageSize = 20;
        private readonly List<(string articleId, string Message, string CreatedAt)> _notifications = [];

        public async Task Render()
        {
            var response = await NotificationService.GetNotifications(pageSharedStorage);
            await ProcessNotificationsResponse(response);

            int totalPages = (_notifications.Count + PageSize - 1) / PageSize;
            int currentPage = 1;

            while (true)
            {
                PageHelper.DisplayHeader();
                PageHelper.DisplaySubHeader("View Notifications");
                Console.WriteLine();

                if (_notifications.Count == 0)
                {
                    await PageHelper.ShowInfoToast("No notifications found.", 1000);
                    break;
                }
                else
                {
                    int start = (currentPage - 1) * PageSize;
                    int end = Math.Min(start + PageSize, _notifications.Count);

                    PageHelper.CenterText(PageHelper.JoinWithSpacing(["ArticleId","Message", "Created At\n"], PageHelper.ConsoleWidth() - 10), color: ConsoleColor.Blue);
                    PageHelper.DrawLine(max: PageHelper.ConsoleWidth(), lineSymbol: '-');
                    Console.WriteLine();

                    for (int i = start; i < end; i++)
                    {
                        var (id, message, createdAt) = _notifications[i];
                        string displayMessage = message.Length > PageHelper.ConsoleWidth() - 35 ? message[..(PageHelper.ConsoleWidth() - 35)] + "..." : message;
                        PageHelper.CenterText(PageHelper.JoinWithSpacing([id, displayMessage, $"{createdAt}\n"], PageHelper.ConsoleWidth() - 10));
                    }

                    Console.WriteLine();
                    PageHelper.CenterText($"Page {currentPage} of {totalPages}", color: ConsoleColor.Blue);
                    Console.WriteLine();
                }

                Console.WriteLine();
                PageHelper.CenterText("N - Next Page | P - Previous Page | V - View Headline | B - Back", color: ConsoleColor.Blue);
                var key = Console.ReadKey(true).Key;
                Console.WriteLine();

                if (key == ConsoleKey.N && currentPage < totalPages)
                    currentPage++;
                else if (key == ConsoleKey.P && currentPage > 1)
                    currentPage--;
                else if (key == ConsoleKey.V)
                    await ViewHeadlineById();
                else if (key == ConsoleKey.B)
                    break;
                else
                    await PageHelper.ShowErrorToast("Invalid choice. Please try again.");
            }
        }

        private async Task ProcessNotificationsResponse(ResponseMessage response)
        {
            _notifications.Clear();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _notifications.AddRange(ParseNotifications(response.Data));
            }
            else
            {
                await PageHelper.ShowErrorToast($"Failed to fetch notifications: {response.Message}", 3000);
            }
        }

        private static List<(string articleId, string Message, string CreatedAt)> ParseNotifications(JsonNode? data)
        {
            var result = new List<(string articleId, string Message, string CreatedAt)>();
            if (data is JsonArray array)
            {
                foreach (var item in array)
                {
                    if (item is JsonObject obj)
                    {
                        string message = "(No Message)";
                        string createdAt = "";
                        var type = obj["type"]?.ToString();
                        var article = obj["article"] as JsonObject;
                        var category = article?["category"] as JsonObject;
                        string articleId = article!["id"]!.ToString();
                        var articleTitle = article?["title"]?.ToString();
                        var categoryName = category?["name"]?.ToString()?.ToUpper();

                        if (type == "Category" && !string.IsNullOrEmpty(articleTitle) && !string.IsNullOrEmpty(categoryName))
                        {
                            message = $"New article in your subscribed category '{categoryName}': \"{articleTitle}\"";
                        }

                        if (DateTime.TryParse(obj["sentAt"]?.ToString(), out var dt))
                        {
                            createdAt = dt.ToString("g");
                        }
                        result.Add((articleId, message, createdAt));
                    }
                }
            }
            return result;
        }
        private async Task ViewHeadlineById()
        {
            Console.WriteLine();
            PageHelper.CenterText("Enter the Id of the headline to view: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out int id))
            {
                await PageHelper.ShowErrorToast("Invalid Id. Please enter a valid number.");
                return;
            }
            pageSharedStorage.ArticleId = id;
            await articleDetailPage.Render();
        }
    }
}